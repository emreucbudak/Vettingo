using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
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
            var repository = Substitute.For<IExamRepository>();
            repository.AddExamAsync(Arg.Any<Exam>()).Returns(Task.CompletedTask);
            repository.SaveChangesAsync().Returns(Task.FromResult(1));
            var handler = new CreateExamCommandHandler(repository, Substitute.For<ILogger<CreateExamCommandHandler>>());
            var request = new CreateExamCommandRequest
            {
                Title = "Backend Exam",
                Subject = "Backend",
                Description = "Backend exam description",
                DurationMinutes = 60,
                PassingScore = 70,
                OwnerType = ExamOwnerType.System
            };

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddExamAsync(Arg.Is<Exam>(exam =>
                exam.Title == request.Title &&
                exam.Subject == request.Subject &&
                exam.PassingScore == request.PassingScore));
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateMultipleChoiceQuestionCommandHandler_When_Exam_Not_Found_Should_Throw_NotFoundException()
        {
            var examRepository = Substitute.For<IExamRepository>();
            var questionRepository = Substitute.For<IQuestionRepository>();
            var examId = Guid.NewGuid();
            examRepository.GetExamByIdAsync(examId).Returns(Task.FromResult<Exam?>(null));
            var handler = new CreateMultipleChoiceQuestionCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateMultipleChoiceQuestionCommandHandler>>());

            Func<Task> action = () => handler.Handle(CreateMultipleChoiceQuestionRequest(examId), CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            await questionRepository.DidNotReceive().AddMultipleChoiceQuestionAsync(Arg.Any<MultipleChoiceQuestion>());
            await questionRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateMultipleChoiceQuestionCommandHandler_Should_Create_Question_With_Weight_And_Options()
        {
            var examRepository = Substitute.For<IExamRepository>();
            var questionRepository = Substitute.For<IQuestionRepository>();
            var exam = CreateExam();
            var request = CreateMultipleChoiceQuestionRequest(exam.Id);
            examRepository.GetExamByIdAsync(exam.Id).Returns(Task.FromResult<Exam?>(exam));
            questionRepository.AddMultipleChoiceQuestionAsync(Arg.Any<MultipleChoiceQuestion>()).Returns(Task.CompletedTask);
            questionRepository.SaveChangesAsync().Returns(Task.FromResult(1));
            var handler = new CreateMultipleChoiceQuestionCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateMultipleChoiceQuestionCommandHandler>>());

            await handler.Handle(request, CancellationToken.None);

            await questionRepository.Received(1).AddMultipleChoiceQuestionAsync(Arg.Is<MultipleChoiceQuestion>(question =>
                question.ExamId == request.ExamId &&
                question.QuestionText == request.QuestionText &&
                question.Weight == request.Weight &&
                question.Options.Count == 2));
            await questionRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetMultipleChoiceQuestionsByExamQueryHandler_Should_Map_Weight_And_Options()
        {
            typeof(GetMultipleChoiceQuestionsByExamQueryResponse).GetProperty("Topic").Should().BeNull();
            typeof(GetMultipleChoiceQuestionsByExamQueryResponse).GetProperty("Point").Should().BeNull();
            var repository = Substitute.For<IQuestionRepository>();
            var examId = Guid.NewGuid();
            var question = CreateMultipleChoiceQuestion(examId);
            repository.GetMultipleChoiceQuestionsByExamIdAsync(examId).Returns(Task.FromResult<IEnumerable<MultipleChoiceQuestion>>([question]));
            var handler = new GetMultipleChoiceQuestionsByExamQueryHandler(repository, Substitute.For<ILogger<GetMultipleChoiceQuestionsByExamQueryHandler>>());

            var response = (await handler.Handle(new GetMultipleChoiceQuestionsByExamQueryRequest { ExamId = examId }, CancellationToken.None)).ToList();

            response.Should().ContainSingle();
            response[0].QuestionText.Should().Be(question.QuestionText);
            response[0].Weight.Should().Be(question.Weight);
            response[0].Options.Select(option => option.DisplayOrder).Should().Equal(1, 2);
        }

        private static CreateMultipleChoiceQuestionCommandRequest CreateMultipleChoiceQuestionRequest(Guid examId)
        {
            return new CreateMultipleChoiceQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = "Which option is correct?",
                Weight = 1.5m,
                DisplayOrder = 1,
                Explanation = "Explanation",
                Options =
                [
                    new MultipleChoiceOptionCommandRequest { OptionText = "A", IsCorrect = true, DisplayOrder = 1 },
                    new MultipleChoiceOptionCommandRequest { OptionText = "B", IsCorrect = false, DisplayOrder = 2 }
                ]
            };
        }

        private static Exam CreateExam()
        {
            Exam exam = new();
            exam.CreateExam("Backend Exam", "Backend", "Backend exam description", 60, 70, ExamOwnerType.System, null, null);
            return exam;
        }

        private static MultipleChoiceQuestion CreateMultipleChoiceQuestion(Guid examId)
        {
            MultipleChoiceQuestion question = new();
            question.CreateQuestion(examId, "Which option is correct?", 1.5m, 1, "Explanation");

            MultipleChoiceOption secondOption = new();
            secondOption.CreateOption(question.Id, "B", false, 2);
            MultipleChoiceOption firstOption = new();
            firstOption.CreateOption(question.Id, "A", true, 1);
            question.AddOption(secondOption);
            question.AddOption(firstOption);

            return question;
        }
    }
}
