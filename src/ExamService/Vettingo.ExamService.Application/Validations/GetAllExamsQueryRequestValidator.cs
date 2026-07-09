using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll;

public sealed class GetAllExamsQueryRequestValidator : AbstractValidator<GetAllExamsQueryRequest>
{
    public GetAllExamsQueryRequestValidator()
    {
        // This query has no input fields to validate.
    }
}

