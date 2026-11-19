using FlashMediator;
using FluentValidation;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Messaging;
using Vettingo.ExamService.Application.UnitOfWork;
using ExamAttemptEntity = Vettingo.ExamService.Domain.Entities.ExamAttempt;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed class CreateExamAttemptCommandHandler(
    IExamUnitOfWork unitOfWork,
    IExamResultPublisher publisher,
    IValidator<CreateExamAttemptCommandRequest> validator) : IRequestHandler<CreateExamAttemptCommandRequest>
{
    public async Task Handle(CreateExamAttemptCommandRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var jobStageExam = await unitOfWork.JobStageExamRepository.GetByIdAsync(request.JobStageExamId)
            ?? throw new NotFoundException("İş ilanına ait sınav bulunamadı.");

        if (!jobStageExam.ExamCandidateList.Contains(request.CandidateId))
        {
            throw new UnauthorizedException("Bu sınava katılamazsınız");
        }

        var exam = await unitOfWork.ExamRepository.GetByIdAsync(jobStageExam.ExamId);

        int score = 0;
        if (request.MultipleQuestionsAnswers is not null)
        {
            foreach (var answer in request.MultipleQuestionsAnswers)
            {
                var question = exam?.MultipleChoiceQuestions
                    .FirstOrDefault(examQuestion => examQuestion.Id == answer.MultipleQuestionId);
                var option = question?.Options
                    .FirstOrDefault(questionOption => questionOption.Id == answer.MultipleChoiceOptionId);

                if (question is not null && option?.IsCorrect == true)
                {
                    score += question.Weight;
                }
            }
        }

        if (request.TrueFalseQuestionAnswers is not null)
        {
            foreach (var answer in request.TrueFalseQuestionAnswers)
            {
                var question = exam?.TrueFalseQuestions
                    .FirstOrDefault(examQuestion => examQuestion.Id == answer.TrueFalseQuestionId);
                var option = question?.Options
                    .FirstOrDefault(questionOption => questionOption.Id == answer.TrueFalseOptionId);

                if (question is not null && option?.IsCorrect == true)
                {
                    score += question.Weight;
                }
            }
        }

        var passed = score >= exam!.PassingScore;
        var attempt = new ExamAttemptEntity();
        attempt.CreateAttempt(jobStageExam, request.CandidateId, score, request.StartedAt, request.CompletedAt, passed);

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
