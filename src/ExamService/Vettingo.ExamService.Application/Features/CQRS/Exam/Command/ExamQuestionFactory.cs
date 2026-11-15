using Vettingo.ExamService.Application.DTOs;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command;

internal static class ExamQuestionFactory
{
    public static Domain.Entities.MultipleChoiceQuestion Create(Guid examId, CreateExamMultipleChoiceQuestionDto request)
    {
        Domain.Entities.MultipleChoiceQuestion question = new();
        question.CreateQuestion(examId, request.QuestionText, request.Weight, request.DisplayOrder);

        foreach (var optionRequest in request.Options)
        {
            Domain.Entities.MultipleChoiceOption option = new();
            option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.Order);
            question.AddOption(option);
        }

        return question;
    }

    public static Domain.Entities.TrueFalseQuestion Create(Guid examId, CreateExamTrueFalseQuestionDto request)
    {
        Domain.Entities.TrueFalseQuestion question = new();
        question.CreateQuestion(examId, request.QuestionText, request.Weight, request.DisplayOrder,
            request.TrueAnswer ?? "True", request.FalseAnswer ?? "False");

        foreach (var optionRequest in request.Options)
        {
            Domain.Entities.TrueFalseOption option = new();
            option.CreateOption(question.Id, optionRequest.Type, optionRequest.IsCorrect);
            question.AddOption(option);
        }

        return question;
    }
}
