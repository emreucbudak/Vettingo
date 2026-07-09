using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Query.GetByExam;

public sealed class GetCodeCompletionQuestionsByExamQueryRequestValidator : AbstractValidator<GetCodeCompletionQuestionsByExamQueryRequest>
{
    public GetCodeCompletionQuestionsByExamQueryRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
    }
}

