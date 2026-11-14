using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Application.CQRS
{
    public class ExamCqrsTests
    {
        [Theory]
        [InlineData(null, null, false)]
        [InlineData(false, false, false)]
        [InlineData(null, false, false)]
        [InlineData(false, null, false)]
        [InlineData(true, null, true)]
        [InlineData(null, true, true)]
        public void CreateExamCommandRequestValidator_Should_Require_At_Least_One_Question(
            bool? hasMultipleChoiceQuestion, bool? hasTrueFalseQuestion, bool expectedValid)
        {
            var request = new CreateExamCommandRequest
            {
                Title = "Backend Exam",
                Description = "Backend exam description",
                DurationMinutes = 60,
                PassingScore = 70,
                MultipleChoiceQuestions = hasMultipleChoiceQuestion switch
                {
                    true => [new CreateExamMultipleChoiceQuestionDto
                    {
                        QuestionText = "Which option is correct?",
                        Weight = 1,
                        DisplayOrder = 1,
                        Options = [new CreateExamMultipleChoiceOptionDto { OptionText = "A", IsCorrect = true }]
                    }],
                    false => [],
                    null => null
                },
                TrueFalseQuestions = hasTrueFalseQuestion switch
                {
                    true => [new CreateExamTrueFalseQuestionDto
                    {
                        QuestionText = "Is this true?",
                        Weight = 1,
                        DisplayOrder = 1,
                        TrueAnswer = "Yes",
                        FalseAnswer = "No"
                    }],
                    false => [],
                    null => null
                }
            };

            new CreateExamCommandRequestValidator().Validate(request).IsValid.Should().Be(expectedValid);
        }

        [Fact]
        public async Task CreateExamCommandHandler_Should_Create_And_Save_Exam()
        {
            var repository = Substitute.For<IExamRepository<Exam>>();
            repository.AddAsync(Arg.Any<Exam>()).Returns(_ => CompleteAsync());
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateExamCommandHandler(repository, Substitute.For<ILogger<CreateExamCommandHandler>>());
            var request = new CreateExamCommandRequest
            {
                Title = "Backend Exam",
                Description = "Backend exam description",
                DurationMinutes = 60,
                PassingScore = 70,
                OwnerType = ExamOwnerType.System
            };

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddAsync(Arg.Is<Exam>(exam =>
                exam.Title == request.Title &&
                exam.PassingScore == request.PassingScore));
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateExamCommandHandler_Should_Save_Questions_And_Options_From_Json()
        {
            var request = JsonSerializer.Deserialize<CreateExamCommandRequest>(
                """
                {
                    "title": "Backend Exam",
                    "description": "Backend exam description",
                    "durationMinutes": 60,
                    "passingScore": 70,
                    "multipleChoiceQuestions": [
                        {
                            "questionText": "Which option is correct?",
                            "weight": 2,
                            "displayOrder": 1,
                            "options": [{ "optionText": "A", "isCorrect": true }]
                        }
                    ],
                    "trueFalseQuestions": [
                        {
                            "questionText": "Is this true?",
                            "weight": 3,
                            "displayOrder": 2,
                            "trueAnswer": "Yes",
                            "falseAnswer": "No"
                        }
                    ]
                }
                """, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
            var repository = Substitute.For<IExamRepository<Exam>>();
            Exam? addedExam = null;
            repository.AddAsync(Arg.Do<Exam>(exam => addedExam = exam)).Returns(Task.CompletedTask);
            var handler = new CreateExamCommandHandler(repository, Substitute.For<ILogger<CreateExamCommandHandler>>());

            await handler.Handle(request, CancellationToken.None);

            addedExam.Should().NotBeNull();
            var exam = addedExam!;
            var multipleChoice = exam.MultipleChoiceQuestions.Should().ContainSingle().Subject;
            multipleChoice.ExamId.Should().Be(exam.Id);
            multipleChoice.QuestionText.Should().Be("Which option is correct?");
            multipleChoice.Weight.Should().Be(2);
            multipleChoice.DisplayOrder.Should().Be(1);
            var option = multipleChoice.Options.Should().ContainSingle().Subject;
            option.MultipleChoiceQuestionId.Should().Be(multipleChoice.Id);
            option.OptionText.Should().Be("A");
            option.IsCorrect.Should().BeTrue();

            var trueFalse = exam.TrueFalseQuestions.Should().ContainSingle().Subject;
            trueFalse.ExamId.Should().Be(exam.Id);
            trueFalse.QuestionText.Should().Be("Is this true?");
            trueFalse.Weight.Should().Be(3);
            trueFalse.DisplayOrder.Should().Be(2);
            trueFalse.TrueAnswer.Should().Be("Yes");
            trueFalse.FalseAnswer.Should().Be("No");
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetMultipleChoiceQuestionsByExamQueryHandler_Should_Map_Weight_And_Options()
        {
            typeof(GetMultipleChoiceQuestionsByExamQueryResponse).GetProperty("Topic").Should().BeNull();
            typeof(GetMultipleChoiceQuestionsByExamQueryResponse).GetProperty("Point").Should().BeNull();
            var repository = Substitute.For<IExamRepository<Exam>>();
            var exam = CreateExam();
            var question = CreateMultipleChoiceQuestion(exam.Id);
            var laterQuestion = new MultipleChoiceQuestion();
            laterQuestion.CreateQuestion(exam.Id, "Later question", 1, 2);
            exam.AddMultipleChoiceQuestion(laterQuestion);
            exam.AddMultipleChoiceQuestion(question);
            repository.GetByIdAsync(exam.Id).Returns(_ => ReturnAsync<Exam?>(exam));
            var handler = new GetMultipleChoiceQuestionsByExamQueryHandler(repository, Substitute.For<ILogger<GetMultipleChoiceQuestionsByExamQueryHandler>>());

            var response = (await handler.Handle(new GetMultipleChoiceQuestionsByExamQueryRequest(exam.Id), CancellationToken.None)).ToList();

            response.Select(item => item.DisplayOrder).Should().Equal(1, 2);
            response[0].QuestionText.Should().Be(question.QuestionText);
            response[0].Weight.Should().Be(question.Weight);
            response[0].Options.Should().BeEquivalentTo(question.Options.Select(option => new
            {
                option.Id,
                option.OptionText,
                option.IsCorrect
            }));
        }

        private static Exam CreateExam()
        {
            Exam exam = new();
            exam.CreateExam("Backend Exam", "Backend exam description", 60, 70, ExamOwnerType.System, null);
            return exam;
        }

        private static MultipleChoiceQuestion CreateMultipleChoiceQuestion(Guid examId)
        {
            MultipleChoiceQuestion question = new();
            question.CreateQuestion(examId, "Which option is correct?", 2, 1);

            MultipleChoiceOption secondOption = new();
            secondOption.CreateOption(question.Id, "B", false);
            MultipleChoiceOption firstOption = new();
            firstOption.CreateOption(question.Id, "A", true);
            question.AddOption(secondOption);
            question.AddOption(firstOption);

            return question;
        }

        private static async Task CompleteAsync()
        {
            await Task.Yield();
        }

        private static async Task<T> ReturnAsync<T>(T value)
        {
            await Task.Yield();
            return value;
        }
    }
}
