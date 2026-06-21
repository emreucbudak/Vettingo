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
                Subject = exam.Subject,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                MultipleChoiceQuestions = exam.MultipleChoiceQuestions
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetExamByIdMultipleChoiceQuestionResponse
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Topic = question.Topic,
                        Point = question.Point,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Explanation = question.Explanation,
                        Options = question.Options
                            .OrderBy(option => option.DisplayOrder)
                            .Select(option => new GetExamByIdMultipleChoiceOptionResponse
                            {
                                Id = option.Id,
                                OptionText = option.OptionText,
                                IsCorrect = option.IsCorrect,
                                DisplayOrder = option.DisplayOrder
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
                        Topic = question.Topic,
                        Point = question.Point,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Explanation = question.Explanation,
                        CorrectAnswer = question.CorrectAnswer
                    })
                    .ToList(),
                ClassicQuestions = exam.ClassicQuestions
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetExamByIdClassicQuestionResponse
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Topic = question.Topic,
                        Point = question.Point,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Explanation = question.Explanation,
                        ExpectedAnswer = question.ExpectedAnswer
                    })
                    .ToList(),
                CodeCompletionQuestions = exam.CodeCompletionQuestions
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetExamByIdCodeCompletionQuestionResponse
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Topic = question.Topic,
                        Point = question.Point,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Explanation = question.Explanation,
                        CodeSnippet = question.CodeSnippet,
                        ExpectedAnswer = question.ExpectedAnswer
                    })
                    .ToList()
            };
        }
    }
}



