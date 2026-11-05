using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.CreateTrueFalseQuestion;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Application.CQRS
{
    public class ExamCqrsTests
    {
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
        public async Task CreateMultipleChoiceQuestionCommandHandler_When_Exam_Not_Found_Should_Throw_NotFoundException()
        {
            var examRepository = Substitute.For<IExamRepository<Exam>>();
            var examId = Guid.NewGuid();
            examRepository.GetByIdAsync(examId).Returns(_ => ReturnAsync<Exam?>(null));
            var handler = new CreateMultipleChoiceQuestionCommandHandler(
                examRepository,
                Substitute.For<ILogger<CreateMultipleChoiceQuestionCommandHandler>>());

            Func<Task> action = () => handler.Handle(CreateMultipleChoiceQuestionRequest(examId), CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            await examRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateMultipleChoiceQuestionCommandHandler_Should_Create_Question_With_Weight_And_Options()
        {
            var examRepository = Substitute.For<IExamRepository<Exam>>();
            var exam = CreateExam();
            var request = CreateMultipleChoiceQuestionRequest(exam.Id);
            examRepository.GetByIdAsync(exam.Id).Returns(_ => ReturnAsync<Exam?>(exam));
            examRepository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateMultipleChoiceQuestionCommandHandler(
                examRepository,
                Substitute.For<ILogger<CreateMultipleChoiceQuestionCommandHandler>>());

            await handler.Handle(request, CancellationToken.None);

            var question = exam.MultipleChoiceQuestions.Should().ContainSingle().Subject;
            question.ExamId.Should().Be(exam.Id);
            question.QuestionText.Should().Be(request.QuestionText);
            question.Weight.Should().Be(request.Weight);
            question.DisplayOrder.Should().Be(request.DisplayOrder);
            question.Options.Should().BeEquivalentTo(request.Options.Select(option => new
            {
                MultipleChoiceQuestionId = question.Id,
                option.OptionText,
                option.IsCorrect
            }));
            await examRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateTrueFalseQuestionCommandHandler_When_Exam_Not_Found_Should_Throw_Without_Saving()
        {
            var examRepository = Substitute.For<IExamRepository<Exam>>();
            var examId = Guid.NewGuid();
            examRepository.GetByIdAsync(examId).Returns(_ => ReturnAsync<Exam?>(null));
            var handler = new CreateTrueFalseQuestionCommandHandler(
                examRepository,
                Substitute.For<ILogger<CreateTrueFalseQuestionCommandHandler>>());

            Func<Task> action = () => handler.Handle(
                new CreateTrueFalseQuestionCommandRequest { ExamId = examId }, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            await examRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateTrueFalseQuestionCommandHandler_Should_Add_Question_To_Exam_And_Save()
        {
            var examRepository = Substitute.For<IExamRepository<Exam>>();
            var exam = CreateExam();
            var request = new CreateTrueFalseQuestionCommandRequest
            {
                ExamId = exam.Id,
                QuestionText = "Is this true?",
                Weight = 2,
                DisplayOrder = 1,
                TrueAnswer = "True",
                FalseAnswer = "False"
            };
            examRepository.GetByIdAsync(exam.Id).Returns(_ => ReturnAsync<Exam?>(exam));
            examRepository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateTrueFalseQuestionCommandHandler(
                examRepository,
                Substitute.For<ILogger<CreateTrueFalseQuestionCommandHandler>>());

            await handler.Handle(request, CancellationToken.None);

            var question = exam.TrueFalseQuestions.Should().ContainSingle().Subject;
            question.ExamId.Should().Be(exam.Id);
            question.QuestionText.Should().Be(request.QuestionText);
            question.Weight.Should().Be(request.Weight);
            question.DisplayOrder.Should().Be(request.DisplayOrder);
            question.TrueAnswer.Should().Be(request.TrueAnswer);
            question.FalseAnswer.Should().Be(request.FalseAnswer);
            await examRepository.Received(1).SaveChangesAsync();
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

        private static CreateMultipleChoiceQuestionCommandRequest CreateMultipleChoiceQuestionRequest(Guid examId)
        {
            return new CreateMultipleChoiceQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = "Which option is correct?",
                Weight = 2,
                DisplayOrder = 1,
                Options =
                [
                    new MultipleChoiceOptionCommandRequest { OptionText = "A", IsCorrect = true },
                    new MultipleChoiceOptionCommandRequest { OptionText = "B", IsCorrect = false }
                ]
            };
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
