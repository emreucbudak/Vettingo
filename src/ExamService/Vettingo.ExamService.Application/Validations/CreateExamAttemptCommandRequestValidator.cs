using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed class CreateExamAttemptCommandRequestValidator : AbstractValidator<CreateExamAttemptCommandRequest>
{
    public CreateExamAttemptCommandRequestValidator()
    {
        RuleFor(request => request.CandidateId).NotEmpty();
        RuleFor(request => request.Score).GreaterThan(0);
        RuleFor(request => request.CompletedAt).GreaterThan(request => request.StartedAt);
    }
}
