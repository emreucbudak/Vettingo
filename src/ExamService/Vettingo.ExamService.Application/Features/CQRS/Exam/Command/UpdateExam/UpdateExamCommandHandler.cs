using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.UnitOfWork;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam
{
    public class UpdateExamCommandHandler(IExamUnitOfWork unitOfWork, ILogger<UpdateExamCommandHandler> logger) : IRequestHandler<UpdateExamCommandRequest>
    {
        public async Task Handle(UpdateExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateExamCommandHandler));
            var exam = await unitOfWork.ExamRepository.GetExamForUpdateAsync(
                request.ExamId, request.CompanyId, cancellationToken);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            if (await unitOfWork.JobStageExamRepository.AnyJobStageExamByExamIdAsync(request.ExamId, cancellationToken))
            {
                throw new BusinessException("Bu sınav şu anda bir iş ilanında kullanıldığı için güncellenemez.");
            }

            exam.UpdateExam(request.Title, request.DurationMinutes, request.PassingScore, request.OwnerType, request.CompanyId);

            if (request.MultipleChoiceQuestions is { Count: > 0 } multipleChoiceRequests)
            {
                var questions = new List<Domain.Entities.MultipleChoiceQuestion>(multipleChoiceRequests.Count);
                foreach (var questionRequest in multipleChoiceRequests)
                {
                    Domain.Entities.MultipleChoiceQuestion question = new();
                    question.CreateQuestion(exam.Id, questionRequest.QuestionText, questionRequest.Weight, questionRequest.DisplayOrder);

                    foreach (var optionRequest in questionRequest.Options)
                    {
                        Domain.Entities.MultipleChoiceOption option = new();
                        option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.Order);
                        question.AddOption(option);
                    }

                    questions.Add(question);
                }

                exam.UpdateMultipleChoiceQuestions(questions);
            }

            if (request.TrueFalseQuestions is { Count: > 0 } trueFalseRequests)
            {
                var questions = new List<Domain.Entities.TrueFalseQuestion>(trueFalseRequests.Count);
                foreach (var questionRequest in trueFalseRequests)
                {
                    Domain.Entities.TrueFalseQuestion question = new();
                    question.CreateQuestion(exam.Id, questionRequest.QuestionText, questionRequest.Weight, questionRequest.DisplayOrder);

                    foreach (var optionRequest in questionRequest.Options)
                    {
                        Domain.Entities.TrueFalseOption option = new();
                        option.CreateOption(question.Id, optionRequest.Type, optionRequest.IsCorrect);
                        question.AddOption(option);
                    }

                    questions.Add(question);
                }

                exam.UpdateTrueFalseQuestions(questions);
            }

            unitOfWork.ExamRepository.Update(exam);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}



