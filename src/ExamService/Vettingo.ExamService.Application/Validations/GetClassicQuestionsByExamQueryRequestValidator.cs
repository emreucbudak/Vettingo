using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Query.GetByExam;

public sealed class GetClassicQuestionsByExamQueryRequestValidator : AbstractValidator<GetClassicQuestionsByExamQueryRequest>
{
    public GetClassicQuestionsByExamQueryRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
    }
}

