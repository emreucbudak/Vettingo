using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
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
    public async Task Question_Query_When_Exam_Is_Missing_Should_Return_Empty_Collection(bool multipleChoice)
    {
        var repository = Substitute.For<IExamRepository<Exam>>();
        var examId = Guid.NewGuid();
        repository.GetByIdAsync(examId).Returns((Exam?)null);

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
        var repository = Substitute.For<IExamRepository<Exam>>();
        repository.GetByIdAsync(exam.Id).Returns(exam);

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
        exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
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
