using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.UpdateTrueFalseQuestion;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public sealed class QuestionAggregateHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Update_Should_Modify_Question_In_Exam_And_Save(bool multipleChoice)
    {
        var exam = CreateExam();
        var repository = Substitute.For<IExamRepository>();
        repository.GetExamByIdAsync(exam.Id).Returns(exam);

        if (multipleChoice)
        {
            var question = exam.MultipleChoiceQuestions.Single();
            var oldOption = question.Options.Single();
            var handler = new UpdateMultipleChoiceQuestionCommandHandler(repository, Logger<UpdateMultipleChoiceQuestionCommandHandler>());
            await handler.Handle(new UpdateMultipleChoiceQuestionCommandRequest
            {
                ExamId = exam.Id,
                QuestionId = question.Id,
                QuestionText = "Updated question",
                Weight = 2,
                DisplayOrder = 3,
                Options = [new MultipleChoiceOptionCommandRequest { OptionText = "B", IsCorrect = true }]
            }, CancellationToken.None);

            question.QuestionText.Should().Be("Updated question");
            question.Weight.Should().Be(2);
            question.DisplayOrder.Should().Be(3);
            var option = question.Options.Should().ContainSingle().Subject;
            option.Id.Should().NotBe(oldOption.Id);
            option.MultipleChoiceQuestionId.Should().Be(question.Id);
            option.OptionText.Should().Be("B");
            option.IsCorrect.Should().BeTrue();
        }
        else
        {
            var question = exam.TrueFalseQuestions.Single();
            var handler = new UpdateTrueFalseQuestionCommandHandler(repository, Logger<UpdateTrueFalseQuestionCommandHandler>());
            await handler.Handle(new UpdateTrueFalseQuestionCommandRequest
            {
                ExamId = exam.Id,
                QuestionId = question.Id,
                QuestionText = "Updated question",
                Weight = 2,
                DisplayOrder = 3,
                TrueAnswer = "Yes",
                FalseAnswer = "No"
            }, CancellationToken.None);

            question.QuestionText.Should().Be("Updated question");
            question.Weight.Should().Be(2);
            question.DisplayOrder.Should().Be(3);
            question.TrueAnswer.Should().Be("Yes");
            question.FalseAnswer.Should().Be("No");
        }

        exam.MultipleChoiceQuestions.Should().ContainSingle();
        exam.TrueFalseQuestions.Should().ContainSingle();
        await repository.Received(1).SaveChangesAsync();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Should_Remove_Only_Selected_Question_And_Save(bool multipleChoice)
    {
        var exam = CreateExam();
        var repository = Substitute.For<IExamRepository>();
        repository.GetExamByIdAsync(exam.Id).Returns(exam);

        if (multipleChoice)
        {
            var questionId = exam.MultipleChoiceQuestions.Single().Id;
            var retainedQuestion = new MultipleChoiceQuestion();
            retainedQuestion.CreateQuestion(exam.Id, "Retained question", 1, 3);
            exam.AddMultipleChoiceQuestion(retainedQuestion);
            var handler = new DeleteMultipleChoiceQuestionCommandHandler(repository, Logger<DeleteMultipleChoiceQuestionCommandHandler>());
            await handler.Handle(new DeleteMultipleChoiceQuestionCommandRequest { ExamId = exam.Id, QuestionId = questionId }, CancellationToken.None);

            exam.MultipleChoiceQuestions.Should().ContainSingle().Which.Should().BeSameAs(retainedQuestion);
        }
        else
        {
            var questionId = exam.TrueFalseQuestions.Single().Id;
            var retainedQuestion = new TrueFalseQuestion();
            retainedQuestion.CreateQuestion(exam.Id, "Retained question", 1, 3, "True", "False");
            exam.AddTrueFalseQuestion(retainedQuestion);
            var handler = new DeleteTrueFalseQuestionCommandHandler(repository, Logger<DeleteTrueFalseQuestionCommandHandler>());
            await handler.Handle(new DeleteTrueFalseQuestionCommandRequest { ExamId = exam.Id, QuestionId = questionId }, CancellationToken.None);

            exam.TrueFalseQuestions.Should().ContainSingle().Which.Should().BeSameAs(retainedQuestion);
        }

        exam.MultipleChoiceQuestions.Should().ContainSingle();
        exam.TrueFalseQuestions.Should().ContainSingle();
        await repository.Received(1).SaveChangesAsync();
    }

    [Theory]
    [InlineData("update-multiple-choice", false)]
    [InlineData("update-multiple-choice", true)]
    [InlineData("delete-multiple-choice", false)]
    [InlineData("delete-multiple-choice", true)]
    [InlineData("update-true-false", false)]
    [InlineData("update-true-false", true)]
    [InlineData("delete-true-false", false)]
    [InlineData("delete-true-false", true)]
    public async Task Mutation_When_Exam_Is_Missing_Or_Question_Belongs_To_Another_Exam_Should_Not_Save(string operation, bool examExists)
    {
        var exam = CreateExam();
        var otherExam = CreateExam();
        var multipleChoiceId = otherExam.MultipleChoiceQuestions.Single().Id;
        var trueFalseId = otherExam.TrueFalseQuestions.Single().Id;
        var repository = Substitute.For<IExamRepository>();
        repository.GetExamByIdAsync(exam.Id).Returns(examExists ? exam : null);

        Func<Task> action = operation switch
        {
            "update-multiple-choice" => () => new UpdateMultipleChoiceQuestionCommandHandler(repository, Logger<UpdateMultipleChoiceQuestionCommandHandler>())
                .Handle(new UpdateMultipleChoiceQuestionCommandRequest { ExamId = exam.Id, QuestionId = multipleChoiceId }, CancellationToken.None),
            "delete-multiple-choice" => () => new DeleteMultipleChoiceQuestionCommandHandler(repository, Logger<DeleteMultipleChoiceQuestionCommandHandler>())
                .Handle(new DeleteMultipleChoiceQuestionCommandRequest { ExamId = exam.Id, QuestionId = multipleChoiceId }, CancellationToken.None),
            "update-true-false" => () => new UpdateTrueFalseQuestionCommandHandler(repository, Logger<UpdateTrueFalseQuestionCommandHandler>())
                .Handle(new UpdateTrueFalseQuestionCommandRequest { ExamId = exam.Id, QuestionId = trueFalseId }, CancellationToken.None),
            "delete-true-false" => () => new DeleteTrueFalseQuestionCommandHandler(repository, Logger<DeleteTrueFalseQuestionCommandHandler>())
                .Handle(new DeleteTrueFalseQuestionCommandRequest { ExamId = exam.Id, QuestionId = trueFalseId }, CancellationToken.None),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        await action.Should().ThrowAsync<NotFoundException>();
        await repository.DidNotReceive().SaveChangesAsync();
        exam.MultipleChoiceQuestions.Should().ContainSingle().Which.QuestionText.Should().Be("Which option?");
        exam.TrueFalseQuestions.Should().ContainSingle().Which.QuestionText.Should().Be("Is this true?");
        otherExam.MultipleChoiceQuestions.Should().ContainSingle();
        otherExam.TrueFalseQuestions.Should().ContainSingle();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Question_Query_When_Exam_Is_Missing_Should_Return_Empty_Collection(bool multipleChoice)
    {
        var repository = Substitute.For<IExamRepository>();
        var examId = Guid.NewGuid();
        repository.GetExamByIdAsync(examId).Returns((Exam?)null);

        if (multipleChoice)
        {
            var result = await new GetMultipleChoiceQuestionsByExamQueryHandler(repository, Logger<GetMultipleChoiceQuestionsByExamQueryHandler>())
                .Handle(new GetMultipleChoiceQuestionsByExamQueryRequest(examId), CancellationToken.None);
            result.Should().BeEmpty();
        }
        else
        {
            var result = await new GetTrueFalseQuestionsByExamQueryHandler(repository, Logger<GetTrueFalseQuestionsByExamQueryHandler>())
                .Handle(new GetTrueFalseQuestionsByExamQueryRequest(examId), CancellationToken.None);
            result.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task TrueFalse_Query_Should_Map_Questions_In_DisplayOrder()
    {
        var exam = CreateExam();
        var firstQuestion = new TrueFalseQuestion();
        firstQuestion.CreateQuestion(exam.Id, "First question", 2, 1, "Yes", "No");
        exam.AddTrueFalseQuestion(firstQuestion);
        var repository = Substitute.For<IExamRepository>();
        repository.GetExamByIdAsync(exam.Id).Returns(exam);

        var result = (await new GetTrueFalseQuestionsByExamQueryHandler(repository, Logger<GetTrueFalseQuestionsByExamQueryHandler>())
            .Handle(new GetTrueFalseQuestionsByExamQueryRequest(exam.Id), CancellationToken.None)).ToList();

        result.Select(question => question.DisplayOrder).Should().Equal(1, 2);
        result[0].Id.Should().Be(firstQuestion.Id);
        result[0].ExamId.Should().Be(exam.Id);
        result[0].QuestionText.Should().Be(firstQuestion.QuestionText);
        result[0].Weight.Should().Be(firstQuestion.Weight);
        result[0].TrueAnswer.Should().Be("Yes");
        result[0].FalseAnswer.Should().Be("No");
    }

    private static ILogger<T> Logger<T>() => Substitute.For<ILogger<T>>();

    private static Exam CreateExam()
    {
        var exam = new Exam();
        exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null, null);
        var multipleChoiceQuestion = new MultipleChoiceQuestion();
        multipleChoiceQuestion.CreateQuestion(exam.Id, "Which option?", 1, 1);
        var option = new MultipleChoiceOption();
        option.CreateOption(multipleChoiceQuestion.Id, "A", true);
        multipleChoiceQuestion.AddOption(option);
        exam.AddMultipleChoiceQuestion(multipleChoiceQuestion);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2, "True", "False");
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        return exam;
    }
}
