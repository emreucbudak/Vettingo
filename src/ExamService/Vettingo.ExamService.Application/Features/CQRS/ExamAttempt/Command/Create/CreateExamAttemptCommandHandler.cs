using FlashMediator;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Messaging;
using Vettingo.ExamService.Application.Repository;
using ExamAttemptEntity = Vettingo.ExamService.Domain.Entities.ExamAttempt;
using JobStageExamEntity = Vettingo.ExamService.Domain.Entities.JobStageExam;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed class CreateExamAttemptCommandHandler(
    IExamRepository<JobStageExamEntity> jobStageExamRepository,
    IExamRepository<ExamAttemptEntity> attemptRepository,
    IExamResultPublisher publisher) : IRequestHandler<CreateExamAttemptCommandRequest>
{
    public async Task Handle(CreateExamAttemptCommandRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var jobStageExam = await jobStageExamRepository.GetByIdAsync(request.JobStageExamId)
            ?? throw new NotFoundException("İş ilanına ait sınav bulunamadı.");

        if (!jobStageExam.ExamCandidateList.Contains(request.CandidateId))
        {
            return;
        }

        var attempt = new ExamAttemptEntity();
        attempt.CreateAttempt(jobStageExam, request.CandidateId, request.Score, request.StartedAt, request.CompletedAt);

        cancellationToken.ThrowIfCancellationRequested();
        jobStageExam.AddAttempt(attempt);
        await attemptRepository.AddAsync(attempt);
        await attemptRepository.SaveChangesAsync();

        if (attempt.Passed)
        {
            await publisher.PublishAsync(new ExamResultMessage
            {
                UserId = attempt.CandidateId,
                JobId = jobStageExam.JobId
            }, cancellationToken);
        }
    }
}
