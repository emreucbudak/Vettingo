using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById;

public sealed class GetExamByIdQueryRequestValidator : AbstractValidator<GetExamByIdQueryRequest>
{
    public GetExamByIdQueryRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
    }
}

