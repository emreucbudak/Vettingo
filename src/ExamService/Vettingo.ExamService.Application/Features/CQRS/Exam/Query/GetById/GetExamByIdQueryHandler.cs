using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public class GetExamByIdQueryHandler(IExamRepository examRepository, ILogger<GetExamByIdQueryHandler> logger) : IRequestHandler<GetExamByIdQueryRequest, GetExamByIdQueryResponse>
    {
        public async Task<GetExamByIdQueryResponse> Handle(GetExamByIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetExamByIdQueryHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            return new GetExamByIdQueryResponse
            {
                Id = exam.Id,
                CompanyId = exam.CompanyId,
                JobId = exam.JobId,
                OwnerType = exam.OwnerType,
                Title = exam.Title,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                MultipleChoiceQuestions = exam.MultipleChoiceQuestions
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetExamByIdMultipleChoiceQuestionResponse
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Options = question.Options
                            .Select(option => new GetExamByIdMultipleChoiceOptionResponse
                            {
                                Id = option.Id,
                                OptionText = option.OptionText,
                                IsCorrect = option.IsCorrect
                            })
                            .ToList()
                    })
                    .ToList(),
                TrueFalseQuestions = exam.TrueFalseQuestions
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetExamByIdTrueFalseQuestionResponse
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        CorrectAnswer = question.CorrectAnswer
                    })
                    .ToList()
            };
        }
    }
}



