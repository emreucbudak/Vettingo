using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam
{
    public class UpdateExamCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<UpdateExamCommandHandler> logger) : IRequestHandler<UpdateExamCommandRequest>
    {
        public async Task Handle(UpdateExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateExamCommandHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            exam.UpdateExam(request.Title, request.Description, request.DurationMinutes, request.PassingScore, request.OwnerType, request.CompanyId);

            if (request.MultipleChoiceQuestions is { Count: > 0 } multipleChoiceRequests)
            {
                var questions = new List<Domain.Entities.MultipleChoiceQuestion>(multipleChoiceRequests.Count);
                foreach (var questionRequest in multipleChoiceRequests)
                {
                    questions.Add(ExamQuestionFactory.Create(exam.Id, questionRequest));
                }

                exam.UpdateMultipleChoiceQuestions(questions);
            }

            if (request.TrueFalseQuestions is { Count: > 0 } trueFalseRequests)
            {
                var questions = new List<Domain.Entities.TrueFalseQuestion>(trueFalseRequests.Count);
                foreach (var questionRequest in trueFalseRequests)
                {
                    questions.Add(ExamQuestionFactory.Create(exam.Id, questionRequest));
                }

                exam.UpdateTrueFalseQuestions(questions);
            }

            examRepository.Update(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}



