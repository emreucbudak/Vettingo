using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;

public sealed class GetMultipleChoiceQuestionsByExamQueryRequestValidator : AbstractValidator<GetMultipleChoiceQuestionsByExamQueryRequest>
{
    public GetMultipleChoiceQuestionsByExamQueryRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
    }
}

