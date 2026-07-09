using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetById;

public sealed class GetInterviewExamByIdQueryRequestValidator : AbstractValidator<GetInterviewExamByIdQueryRequest>
{
    public GetInterviewExamByIdQueryRequestValidator()
    {
        RuleFor(x => x.InterviewExamId).NotEmpty();
    }
}

