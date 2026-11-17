using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.UnitTests.Repository;

public sealed class ExamQuestionTrackingTests
{
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task Updating_Exam_Should_Replace_Only_Nonempty_Question_Lists(bool replaceMultipleChoice, bool replaceTrueFalse)
    {
        var options = new DbContextOptionsBuilder<ExamDbContext>()
            .UseNpgsql("Host=localhost;Database=exam_tracking_test;Username=test;Password=test")
            .Options;
        using var context = new ExamDbContext(options);
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.System, null);
        var oldMultipleChoice = new MultipleChoiceQuestion();
        oldMultipleChoice.CreateQuestion(exam.Id, "Old multiple choice", 1, 1);
        var oldOption = new MultipleChoiceOption();
        oldOption.CreateOption(oldMultipleChoice.Id, "Old option", true);
        oldMultipleChoice.AddOption(oldOption);
        exam.AddMultipleChoiceQuestion(oldMultipleChoice);
        var oldTrueFalse = new TrueFalseQuestion();
        oldTrueFalse.CreateQuestion(exam.Id, "Old true/false", 1, 2, "Yes", "No");
        var oldTrueFalseOption = new TrueFalseOption();
        oldTrueFalseOption.CreateOption(oldTrueFalse.Id, TrueFalseOptionType.True, true);
        oldTrueFalse.AddOption(oldTrueFalseOption);
        exam.AddTrueFalseQuestion(oldTrueFalse);
        context.Attach(exam);

        var repository = Substitute.For<IExamRepository<Exam>>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        unitOfWork.ExamRepository.Returns(repository);
        repository.GetByIdAsync(exam.Id).Returns(exam);
        repository.When(x => x.Update(exam)).Do(_ => context.Exams.Update(exam));
        var handler = new UpdateExamCommandHandler(unitOfWork, NullLogger<UpdateExamCommandHandler>.Instance);
        var request = new UpdateExamCommandRequest
        {
            ExamId = exam.Id,
            Title = "Updated exam",
            DurationMinutes = exam.DurationMinutes,
            PassingScore = exam.PassingScore,
            MultipleChoiceQuestions = replaceMultipleChoice
                ? [new UpdateExamMultipleChoiceQuestionDto
                {
                    QuestionText = "New multiple choice",
                    Weight = 2,
                    DisplayOrder = 1,
                    Options = [new UpdateExamMultipleChoiceOptionDto
                    {
                        OptionText = "New option", IsCorrect = true, Order = MultipleChoiceOptionOrder.C
                    }]
                }]
                : [],
            TrueFalseQuestions = replaceTrueFalse
                ? [new UpdateExamTrueFalseQuestionDto
                {
                    QuestionText = "New true/false",
                    Weight = 2,
                    DisplayOrder = 2,
                    Options =
                    [
                        new UpdateExamTrueFalseOptionDto { Type = TrueFalseOptionType.True, IsCorrect = true },
                        new UpdateExamTrueFalseOptionDto { Type = TrueFalseOptionType.False, IsCorrect = false }
                    ]
                }]
                : null
        };

        new UpdateExamCommandRequestValidator().Validate(request).IsValid.Should().BeTrue();
        await handler.Handle(request, CancellationToken.None);
        context.ChangeTracker.DetectChanges();

        var multipleChoice = exam.MultipleChoiceQuestions.Should().ContainSingle().Subject;
        var option = multipleChoice.Options.Should().ContainSingle().Subject;
        var trueFalse = exam.TrueFalseQuestions.Should().ContainSingle().Subject;
        exam.Title.Should().Be(request.Title);
        multipleChoice.ExamId.Should().Be(exam.Id);
        option.MultipleChoiceQuestionId.Should().Be(multipleChoice.Id);
        trueFalse.ExamId.Should().Be(exam.Id);
        context.Entry(oldMultipleChoice).State.Should().Be(replaceMultipleChoice ? EntityState.Deleted : EntityState.Unchanged);
        context.Entry(oldOption).State.Should().Be(replaceMultipleChoice ? EntityState.Deleted : EntityState.Unchanged);
        context.Entry(multipleChoice).State.Should().Be(replaceMultipleChoice ? EntityState.Added : EntityState.Unchanged);
        context.Entry(option).State.Should().Be(replaceMultipleChoice ? EntityState.Added : EntityState.Unchanged);
        context.Entry(oldTrueFalse).State.Should().Be(replaceTrueFalse ? EntityState.Deleted : EntityState.Unchanged);
        context.Entry(trueFalse).State.Should().Be(replaceTrueFalse ? EntityState.Added : EntityState.Unchanged);
        option.Order.Should().Be(replaceMultipleChoice ? MultipleChoiceOptionOrder.C : MultipleChoiceOptionOrder.A);
        context.Entry(oldTrueFalseOption).State.Should().Be(replaceTrueFalse ? EntityState.Deleted : EntityState.Unchanged);
        trueFalse.Options.Should().HaveCount(replaceTrueFalse ? 2 : 1);
        foreach (var trueFalseOption in trueFalse.Options)
        {
            trueFalseOption.TrueFalseQuestionId.Should().Be(trueFalse.Id);
            context.Entry(trueFalseOption).State.Should().Be(replaceTrueFalse ? EntityState.Added : EntityState.Unchanged);
        }
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public void Adding_Questions_To_Tracked_Exam_Should_Track_New_Questions_And_Options_As_Added()
    {
        var options = new DbContextOptionsBuilder<ExamDbContext>()
            .UseNpgsql("Host=localhost;Database=exam_tracking_test;Username=test;Password=test")
            .Options;
        using var context = new ExamDbContext(options);
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.System, null);
        context.Attach(exam);

        var multipleChoiceQuestion = new MultipleChoiceQuestion();
        multipleChoiceQuestion.CreateQuestion(exam.Id, "Which option?", 1, 1);
        var option = new MultipleChoiceOption();
        option.CreateOption(multipleChoiceQuestion.Id, "A", true);
        multipleChoiceQuestion.AddOption(option);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2, "True", "False");

        exam.AddMultipleChoiceQuestion(multipleChoiceQuestion);
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        context.ChangeTracker.DetectChanges();

        context.Entry(exam).State.Should().Be(EntityState.Unchanged);
        context.Entry(multipleChoiceQuestion).State.Should().Be(EntityState.Added);
        context.Entry(option).State.Should().Be(EntityState.Added);
        context.Entry(trueFalseQuestion).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public void Removing_Questions_From_Tracked_Exam_Should_Delete_Questions_And_Options()
    {
        var options = new DbContextOptionsBuilder<ExamDbContext>()
            .UseNpgsql("Host=localhost;Database=exam_tracking_test;Username=test;Password=test")
            .Options;
        using var context = new ExamDbContext(options);
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.System, null);
        var multipleChoiceQuestion = new MultipleChoiceQuestion();
        multipleChoiceQuestion.CreateQuestion(exam.Id, "Which option?", 1, 1);
        var option = new MultipleChoiceOption();
        option.CreateOption(multipleChoiceQuestion.Id, "A", true);
        multipleChoiceQuestion.AddOption(option);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2, "True", "False");
        exam.AddMultipleChoiceQuestion(multipleChoiceQuestion);
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        context.Attach(exam);

        exam.RemoveMultipleChoiceQuestion(multipleChoiceQuestion.Id).Should().BeTrue();
        exam.RemoveTrueFalseQuestion(trueFalseQuestion.Id).Should().BeTrue();
        context.ChangeTracker.DetectChanges();

        context.Entry(exam).State.Should().Be(EntityState.Unchanged);
        context.Entry(multipleChoiceQuestion).State.Should().Be(EntityState.Deleted);
        context.Entry(option).State.Should().Be(EntityState.Deleted);
        context.Entry(trueFalseQuestion).State.Should().Be(EntityState.Deleted);
    }

    [Fact]
    public void Updating_Tracked_Question_Should_Replace_Options_Without_Updating_Exam()
    {
        var options = new DbContextOptionsBuilder<ExamDbContext>()
            .UseNpgsql("Host=localhost;Database=exam_tracking_test;Username=test;Password=test")
            .Options;
        using var context = new ExamDbContext(options);
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.System, null);
        var question = new MultipleChoiceQuestion();
        question.CreateQuestion(exam.Id, "Which option?", 1, 1);
        var oldOption = new MultipleChoiceOption();
        oldOption.CreateOption(question.Id, "A", true);
        question.AddOption(oldOption);
        exam.AddMultipleChoiceQuestion(question);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2, "True", "False");
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        context.Attach(exam);

        question.UpdateQuestion("Updated question", 2, 2);
        question.ClearOptions();
        var newOption = new MultipleChoiceOption();
        newOption.CreateOption(question.Id, "B", true);
        question.AddOption(newOption);
        trueFalseQuestion.UpdateQuestion("Updated true/false", 2, 1, "Yes", "No");
        context.ChangeTracker.DetectChanges();

        context.Entry(exam).State.Should().Be(EntityState.Unchanged);
        context.Entry(question).State.Should().Be(EntityState.Modified);
        context.Entry(oldOption).State.Should().Be(EntityState.Deleted);
        context.Entry(newOption).State.Should().Be(EntityState.Added);
        context.Entry(trueFalseQuestion).State.Should().Be(EntityState.Modified);
    }
}
