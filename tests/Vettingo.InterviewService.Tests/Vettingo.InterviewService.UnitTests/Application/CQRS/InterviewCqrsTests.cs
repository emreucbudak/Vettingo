using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Command.CreateInterviewAnswer;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Domain.Enums;
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
            repository.AddAsync(Arg.Any<InterviewQuestionEntity>()).Returns(_ => CompleteAsync());
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
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
        public async Task CreateInterviewExamCommandHandler_When_AI_QuestionIds_Empty_Should_Throw_BadRequestException()
        {
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var questionRepository = Substitute.For<IRepository<InterviewQuestionEntity>>();
            var handler = new CreateInterviewExamCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateInterviewExamCommandHandler>>());
            var request = CreateInterviewRequest(InterviewType.AI, []);

            Func<Task> action = () => handler.Handle(request, CancellationToken.None);

            await action.Should().ThrowAsync<BadRequestException>();
            await examRepository.DidNotReceive().AddAsync(Arg.Any<InterviewExamEntity>());
            await examRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateInterviewExamCommandHandler_Should_Create_AI_Interview_With_Distinct_Questions()
        {
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var questionRepository = Substitute.For<IRepository<InterviewQuestionEntity>>();
            var firstQuestionId = Guid.NewGuid();
            var secondQuestionId = Guid.NewGuid();
            questionRepository.CountAsync(Arg.Any<Expression<Func<InterviewQuestionEntity, bool>>>()).Returns(_ => ReturnAsync(2));
            examRepository.AddAsync(Arg.Any<InterviewExamEntity>()).Returns(_ => CompleteAsync());
            examRepository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateInterviewExamCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateInterviewExamCommandHandler>>());
            var request = CreateInterviewRequest(
                InterviewType.AI,
                [firstQuestionId, firstQuestionId, secondQuestionId]);

            await handler.Handle(request, CancellationToken.None);

            await examRepository.Received(1).AddAsync(Arg.Is<InterviewExamEntity>(exam =>
                exam.CompanyId == request.CompanyId &&
                exam.CandidateId == request.CandidateId &&
                exam.Type == InterviewType.AI &&
                exam.EndDate == request.EndDate &&
                exam.Questions.Count == 2));
            await examRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateInterviewExamCommandHandler_Should_Create_Human_Interview_Without_Questions()
        {
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var questionRepository = Substitute.For<IRepository<InterviewQuestionEntity>>();
            examRepository.AddAsync(Arg.Any<InterviewExamEntity>()).Returns(_ => CompleteAsync());
            examRepository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateInterviewExamCommandHandler(
                examRepository,
                questionRepository,
                Substitute.For<ILogger<CreateInterviewExamCommandHandler>>());
            var request = CreateInterviewRequest(InterviewType.Human, []);

            await handler.Handle(request, CancellationToken.None);

            await examRepository.Received(1).AddAsync(Arg.Is<InterviewExamEntity>(exam =>
                exam.Type == InterviewType.Human &&
                exam.CandidateId == request.CandidateId &&
                exam.Questions.Count == 0));
            await questionRepository.DidNotReceive().CountAsync(Arg.Any<Expression<Func<InterviewQuestionEntity, bool>>>());
        }

        [Fact]
        public async Task CreateInterviewAnswerCommandHandler_When_Exam_Not_Found_Should_Throw_NotFoundException()
        {
            var answerRepository = Substitute.For<IRepository<InterviewAnswerEntity>>();
            var examRepository = Substitute.For<IRepository<InterviewExamEntity>>();
            var examId = Guid.NewGuid();
            examRepository.AnyAsync(Arg.Any<Expression<Func<InterviewExamEntity, bool>>>()).Returns(_ => ReturnAsync(false));
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

        private static CreateInterviewExamCommandRequest CreateInterviewRequest(
            InterviewType type,
            List<Guid> questionIds)
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            return new CreateInterviewExamCommandRequest
            {
                CompanyId = Guid.NewGuid(),
                CandidateId = Guid.NewGuid(),
                Title = type == InterviewType.AI ? "AI Backend Interview" : "Human Backend Interview",
                Description = "Backend interview description",
                Type = type,
                StartDate = startDate,
                EndDate = type == InterviewType.AI ? startDate.AddHours(2) : null,
                QuestionIds = questionIds
            };
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
