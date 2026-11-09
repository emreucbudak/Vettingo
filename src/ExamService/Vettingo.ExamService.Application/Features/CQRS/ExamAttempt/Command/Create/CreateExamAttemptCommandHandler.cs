using FlashMediator;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Messaging;
using Vettingo.ExamService.Application.Repository;
using ExamAttemptEntity = Vettingo.ExamService.Domain.Entities.ExamAttempt;
using JobExamEntity = Vettingo.ExamService.Domain.Entities.JobExam;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed class CreateExamAttemptCommandHandler(
    IExamRepository<JobExamEntity> jobExamRepository,
    IExamRepository<ExamAttemptEntity> attemptRepository,
    IExamResultPublisher publisher) : IRequestHandler<CreateExamAttemptCommandRequest>
{
    public async Task Handle(CreateExamAttemptCommandRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var jobExam = await jobExamRepository.GetByIdAsync(request.JobExamId)
            ?? throw new NotFoundException("İş ilanına ait sınav bulunamadı.");

        var attempt = new ExamAttemptEntity();
        attempt.CreateAttempt(jobExam, request.CandidateId, request.Score, request.StartedAt, request.CompletedAt);

        cancellationToken.ThrowIfCancellationRequested();
        await attemptRepository.AddAsync(attempt);
        await attemptRepository.SaveChangesAsync();

        if (attempt.Passed)
        {
            await publisher.PublishAsync(new ExamResultMessage
            {
                UserId = attempt.CandidateId,
                JobId = jobExam.JobId
            }, cancellationToken);
        }
    }
}
