using FlashMediator;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Messaging;
using Vettingo.ExamService.Application.UnitOfWork;
using ExamAttemptEntity = Vettingo.ExamService.Domain.Entities.ExamAttempt;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed class CreateExamAttemptCommandHandler(
    IExamUnitOfWork unitOfWork,
    IExamResultPublisher publisher) : IRequestHandler<CreateExamAttemptCommandRequest>
{
    public async Task Handle(CreateExamAttemptCommandRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var jobStageExam = await unitOfWork.JobStageExamRepository.GetByIdAsync(request.JobStageExamId)
            ?? throw new NotFoundException("İş ilanına ait sınav bulunamadı.");

        if (!jobStageExam.ExamCandidateList.Contains(request.CandidateId))
        {
            return;
        }

        var attempt = new ExamAttemptEntity();
        attempt.CreateAttempt(jobStageExam, request.CandidateId, request.Score, request.StartedAt, request.CompletedAt);

        cancellationToken.ThrowIfCancellationRequested();
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            jobStageExam.AddAttempt(attempt);
            await unitOfWork.ExamAttemptRepository.AddAsync(attempt);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (attempt.Passed)
            {
                await publisher.PublishAsync(new ExamResultMessage
                {
                    UserId = attempt.CandidateId,
                    JobId = jobStageExam.JobId
                }, cancellationToken);
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
