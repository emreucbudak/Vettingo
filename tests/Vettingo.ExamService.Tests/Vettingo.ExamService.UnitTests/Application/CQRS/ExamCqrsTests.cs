using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Application.UnitOfWork;
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
                DurationMinutes = 60,
                PassingScore = 70,
                MultipleChoiceQuestions = hasMultipleChoiceQuestion switch
                {
                    true => [new CreateExamMultipleChoiceQuestionDto
                    {
                        QuestionText = "Which option is correct?",
                        Weight = 1,
                        DisplayOrder = 1,
                        Options = [new CreateExamMultipleChoiceOptionDto
                        {
                            OptionText = "A", IsCorrect = true, Order = MultipleChoiceOptionOrder.A
                        }]
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
                        Options =
                        [
                            new CreateExamTrueFalseOptionDto { Type = TrueFalseOptionType.True, IsCorrect = true },
                            new CreateExamTrueFalseOptionDto { Type = TrueFalseOptionType.False }
                        ]
                    }],
                    false => [],
                    null => null
                }
            };

            new CreateExamCommandRequestValidator().Validate(request).IsValid.Should().Be(expectedValid);
        }

        [Theory]
        [InlineData("""{"multipleChoiceQuestions":[{"questionText":"Question","weight":1,"displayOrder":1,"options":null}]}""", "MultipleChoiceQuestions[0].Options")]
        [InlineData("""{"multipleChoiceQuestions":[{"questionText":"Question","weight":1,"displayOrder":1,"options":[{"optionText":"Answer"}]}]}""", "MultipleChoiceQuestions[0].Options[0].Order")]
        [InlineData("""{"trueFalseQuestions":[{"questionText":"Question","weight":1,"displayOrder":1,"options":null}]}""", "TrueFalseQuestions[0].Options")]
        [InlineData("""{"trueFalseQuestions":[{"questionText":"Question","weight":1,"displayOrder":1,"options":[{"isCorrect":true},{"type":2}]}]}""", "TrueFalseQuestions[0].Options")]
        public void Exam_Validators_Should_Require_Options_And_Valid_Enum_Values(string json, string propertyName)
        {
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var request = JsonSerializer.Deserialize<CreateExamCommandRequest>(
                json, jsonOptions)! with
            {
                Title = "Exam",
                DurationMinutes = 30
            };

            var updateRequest = JsonSerializer.Deserialize<UpdateExamCommandRequest>(json, jsonOptions)! with
            {
                ExamId = Guid.NewGuid(),
                Title = request.Title,
                DurationMinutes = request.DurationMinutes
            };

            new CreateExamCommandRequestValidator().Validate(request).Errors
                .Should().Contain(error => error.PropertyName == propertyName);
            new UpdateExamCommandRequestValidator().Validate(updateRequest).Errors
                .Should().Contain(error => error.PropertyName == propertyName);
        }

        [Fact]
        public async Task CreateExamCommandHandler_Should_Create_And_Save_Exam()
        {
            var repository = Substitute.For<IExamRepository<Exam>>();
            var unitOfWork = Substitute.For<IExamUnitOfWork>();
            unitOfWork.ExamRepository.Returns(repository);
            repository.AddAsync(Arg.Any<Exam>()).Returns(_ => CompleteAsync());
            var handler = new CreateExamCommandHandler(unitOfWork, Substitute.For<ILogger<CreateExamCommandHandler>>());
            var request = new CreateExamCommandRequest
            {
                Title = "Backend Exam",
                DurationMinutes = 60,
                PassingScore = 70,
                OwnerType = ExamOwnerType.System
            };

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddAsync(Arg.Is<Exam>(exam =>
                exam.Title == request.Title &&
                exam.PassingScore == request.PassingScore));
            await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task CreateExamCommandHandler_Should_Save_Questions_And_Options_From_Json()
        {
            var request = JsonSerializer.Deserialize<CreateExamCommandRequest>(
                """
                {
                    "title": "Backend Exam",
                    "durationMinutes": 60,
                    "passingScore": 70,
                    "multipleChoiceQuestions": [
                        {
                            "questionText": "Which option is correct?",
                            "weight": 2,
                            "displayOrder": 1,
                            "options": [{ "optionText": "A", "isCorrect": true, "order": 2 }]
                        }
                    ],
                    "trueFalseQuestions": [
                        {
                            "questionText": "Is this true?",
                            "weight": 3,
                            "displayOrder": 2,
                            "options": [{ "type": 1, "isCorrect": false }, { "type": 2, "isCorrect": true }]
                        }
                    ]
                }
                """, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
            var repository = Substitute.For<IExamRepository<Exam>>();
            var unitOfWork = Substitute.For<IExamUnitOfWork>();
            unitOfWork.ExamRepository.Returns(repository);
            Exam? addedExam = null;
            repository.AddAsync(Arg.Do<Exam>(exam => addedExam = exam)).Returns(Task.CompletedTask);
            var handler = new CreateExamCommandHandler(unitOfWork, Substitute.For<ILogger<CreateExamCommandHandler>>());

            new CreateExamCommandRequestValidator().Validate(request).IsValid.Should().BeTrue();
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
            option.Order.Should().Be(MultipleChoiceOptionOrder.B);

            var trueFalse = exam.TrueFalseQuestions.Should().ContainSingle().Subject;
            trueFalse.ExamId.Should().Be(exam.Id);
            trueFalse.QuestionText.Should().Be("Is this true?");
            trueFalse.Weight.Should().Be(3);
            trueFalse.DisplayOrder.Should().Be(2);
            trueFalse.Options.Should().OnlyContain(option => option.TrueFalseQuestionId == trueFalse.Id);
            trueFalse.Options.Should().ContainSingle(option => option.Type == TrueFalseOptionType.True && !option.IsCorrect);
            trueFalse.Options.Should().ContainSingle(option => option.Type == TrueFalseOptionType.False && option.IsCorrect);
            await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        private static async Task CompleteAsync()
        {
            await Task.Yield();
        }
    }
}
