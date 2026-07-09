using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam;

public sealed class GetTrueFalseQuestionsByExamQueryRequestValidator : AbstractValidator<GetTrueFalseQuestionsByExamQueryRequest>
{
    public GetTrueFalseQuestionsByExamQueryRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
    }
}

