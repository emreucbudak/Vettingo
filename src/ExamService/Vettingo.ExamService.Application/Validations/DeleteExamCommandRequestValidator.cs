using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam;

public sealed class DeleteExamCommandRequestValidator : AbstractValidator<DeleteExamCommandRequest>
{
    public DeleteExamCommandRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
    }
}

