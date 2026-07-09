using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.DeleteInterviewExam;

public sealed class DeleteInterviewExamCommandRequestValidator : AbstractValidator<DeleteInterviewExamCommandRequest>
{
    public DeleteInterviewExamCommandRequestValidator()
    {
        RuleFor(x => x.InterviewExamId).NotEmpty();
    }
}

