using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Command.CreateInterviewAnswer;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion;
using Vettingo.InterviewService.Application.Repository;
using InterviewAnswerEntity = Vettingo.InterviewService.Domain.Entities.InterviewAnswer;
using InterviewExamEntity = Vettingo.InterviewService.Domain.Entities.InterviewExam;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS
{
    public class InterviewCqrsTests
    {
        [Fact]
        public async Task CreateInterviewQuestionCommandHandler_Should_Create_And_Save_Question()
        {
            var repository = Substitute.For<IRepository<InterviewQuestionEntity>>();
            repository.AddAsync(Arg.Any<InterviewQuestionEntity>()).Returns(Task.CompletedTask);
            repository.SaveChangesAsync().Returns(Task.FromResult(1));
            var handler = new CreateInterviewQuestionCommandHandler(repository, Substitute.For<ILogger<CreateInterviewQuestionCommandHandler>>());
            var request = new CreateInterviewQuestionCommandRequest
            {
                CompanyId = Guid.NewGuid(),
                QuestionText = "Tell us about your last project."
            };

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddAsync(Arg.Is<InterviewQuestionEntity>(question =>
                question.CompanyId == request.CompanyId &&
                question.QuestionText == request.QuestionText));
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateInterviewExamCommandHandler_When_QuestionIds_Empty_Should_Throw_BadRequestException()
        {
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var questionRepository = Substitute.For<IRepository<InterviewQuestionEntity>>();
            var handler = new CreateInterviewExamCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateInterviewExamCommandHandler>>());
            var request = new CreateInterviewExamCommandRequest
            {
                CompanyId = Guid.NewGuid(),
                Title = "Backend Interview",
                Description = "Backend interview description",
                QuestionIds = []
            };

            Func<Task> action = () => handler.Handle(request, CancellationToken.None);

            await action.Should().ThrowAsync<BadRequestException>();
            await examRepository.DidNotReceive().AddAsync(Arg.Any<InterviewExamEntity>());
            await examRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateInterviewExamCommandHandler_Should_Create_Exam_With_Distinct_Questions()
        {
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var questionRepository = Substitute.For<IRepository<InterviewQuestionEntity>>();
            var firstQuestionId = Guid.NewGuid();
            var secondQuestionId = Guid.NewGuid();
            questionRepository.CountAsync(Arg.Any<Expression<Func<InterviewQuestionEntity, bool>>>()).Returns(Task.FromResult(2));
            examRepository.AddAsync(Arg.Any<InterviewExamEntity>()).Returns(Task.CompletedTask);
            examRepository.SaveChangesAsync().Returns(Task.FromResult(1));
            var handler = new CreateInterviewExamCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateInterviewExamCommandHandler>>());
            var request = new CreateInterviewExamCommandRequest
            {
                CompanyId = Guid.NewGuid(),
                Title = "Backend Interview",
                Description = "Backend interview description",
                QuestionIds = [firstQuestionId, firstQuestionId, secondQuestionId]
            };

            await handler.Handle(request, CancellationToken.None);

            await examRepository.Received(1).AddAsync(Arg.Is<InterviewExamEntity>(exam =>
                exam.CompanyId == request.CompanyId &&
                exam.Title == request.Title &&
                exam.Questions.Count == 2));
            await examRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateInterviewAnswerCommandHandler_When_Exam_Not_Found_Should_Throw_NotFoundException()
        {
            var answerRepository = Substitute.For<IRepository<InterviewAnswerEntity>>();
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var examId = Guid.NewGuid();
            examRepository.AnyAsync(exam => exam.Id == examId).Returns(Task.FromResult(false));
            var handler = new CreateInterviewAnswerCommandHandler(
                answerRepository,
                examRepository,
                Substitute.For<ILogger<CreateInterviewAnswerCommandHandler>>());

            Func<Task> action = () => handler.Handle(new CreateInterviewAnswerCommandRequest
            {
                UserId = Guid.NewGuid(),
                InterviewExamId = examId,
                AnswerDate = DateOnly.FromDateTime(DateTime.UtcNow)
            }, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            await answerRepository.DidNotReceive().AddAsync(Arg.Any<InterviewAnswerEntity>());
            await answerRepository.DidNotReceive().SaveChangesAsync();
        }
    }
}
