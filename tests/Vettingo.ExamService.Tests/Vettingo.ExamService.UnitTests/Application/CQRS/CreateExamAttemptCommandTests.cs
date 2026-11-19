using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;
using Vettingo.ExamService.Application.Messaging;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public sealed class CreateExamAttemptCommandTests
{
    [Fact]
    public async Task Should_Reject_Invalid_Attempt_Before_Accessing_Repository()
    {
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        var handler = new CreateExamAttemptCommandHandler(unitOfWork, Substitute.For<IExamResultPublisher>(),
            new CreateExamAttemptCommandRequestValidator());
        var startedAt = DateTime.UtcNow;
        var request = new CreateExamAttemptCommandRequest
        {
            JobStageExamId = Guid.NewGuid(),
            CandidateId = Guid.Empty,
            StartedAt = startedAt,
            CompletedAt = startedAt
        };

        var action = () => handler.Handle(request, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Select(error => error.PropertyName).Should().BeEquivalentTo(
            nameof(request.CandidateId), nameof(request.CompletedAt));
        unitOfWork.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(69, false, false)]
    [InlineData(70, true, false)]
    [InlineData(70, true, true)]
    public async Task Should_Save_Attempt_And_Publish_Only_Passed_Results(int score, bool passed, bool publishFails)
    {
        await using var context = new ExamDbContext(new DbContextOptionsBuilder<ExamDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.System, null);
        var multipleChoiceQuestion = new MultipleChoiceQuestion();
        multipleChoiceQuestion.CreateQuestion(exam.Id, "Choose the correct option", 69, 1);
        var multipleChoiceOption = new MultipleChoiceOption();
        multipleChoiceOption.CreateOption(multipleChoiceQuestion.Id, "Correct option", true);
        multipleChoiceQuestion.AddOption(multipleChoiceOption);
        exam.AddMultipleChoiceQuestion(multipleChoiceQuestion);

        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2);
        var trueOption = new TrueFalseOption();
        trueOption.CreateOption(trueFalseQuestion.Id, TrueFalseOptionType.True, true);
        var falseOption = new TrueFalseOption();
        falseOption.CreateOption(trueFalseQuestion.Id, TrueFalseOptionType.False, false);
        trueFalseQuestion.AddOption(trueOption);
        trueFalseQuestion.AddOption(falseOption);
        exam.AddTrueFalseQuestion(trueFalseQuestion);

        var candidateId = Guid.NewGuid();
        var otherCandidateId = Guid.NewGuid();
        var jobExam = new JobStageExam { Exam = exam };
        jobExam.CreateJobStageExam(Guid.NewGuid(), Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9),
            [candidateId, otherCandidateId]);
        context.JobExams.Add(jobExam);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var publisher = Substitute.For<IExamResultPublisher>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        unitOfWork.ExamRepository.Returns(new ExamRepository<Exam>(context));
        unitOfWork.JobStageExamRepository.Returns(new ExamRepository<JobStageExam>(context));
        unitOfWork.ExamAttemptRepository.Returns(new ExamRepository<ExamAttempt>(context));
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(call => context.SaveChangesAsync(call.Arg<CancellationToken>()));
        var handler = new CreateExamAttemptCommandHandler(unitOfWork, publisher, new CreateExamAttemptCommandRequestValidator());
        var completedAt = DateTime.UtcNow;
        var request = new CreateExamAttemptCommandRequest
        {
            JobStageExamId = jobExam.Id,
            CandidateId = candidateId,
            MultipleQuestionsAnswers =
            [
                new MultipleQuestionsAnswersDto
                {
                    MultipleQuestionId = multipleChoiceQuestion.Id,
                    MultipleChoiceOptionId = multipleChoiceOption.Id
                }
            ],
            TrueFalseQuestionAnswers =
            [
                new TrueFalseQuestionAnswersDto
                {
                    TrueFalseQuestionId = trueFalseQuestion.Id,
                    TrueFalseOptionId = score == 70 ? trueOption.Id : falseOption.Id
                }
            ],
            StartedAt = completedAt.AddMinutes(-20),
            CompletedAt = completedAt
        };

        var submitAsUnlistedCandidate = () =>
            handler.Handle(request with { CandidateId = Guid.NewGuid() }, CancellationToken.None);
        await submitAsUnlistedCandidate.Should().ThrowAsync<UnauthorizedException>();
        (await context.ExamAttempts.AnyAsync()).Should().BeFalse();
        await publisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);

        if (publishFails)
        {
            publisher.PublishAsync(Arg.Any<ExamResultMessage>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new InvalidOperationException("Publish failed.")));

            var action = () => handler.Handle(request, CancellationToken.None);
            await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Publish failed.");

            Received.InOrder(() =>
            {
                _ = unitOfWork.BeginTransactionAsync(CancellationToken.None);
                _ = unitOfWork.SaveChangesAsync(CancellationToken.None);
                _ = publisher.PublishAsync(Arg.Any<ExamResultMessage>(), CancellationToken.None);
                _ = unitOfWork.RollbackTransactionAsync();
            });
            await unitOfWork.DidNotReceiveWithAnyArgs().CommitTransactionAsync(default);
            return;
        }

        await handler.Handle(request, CancellationToken.None);
        context.ChangeTracker.Clear();
        var saved = await context.ExamAttempts.AsNoTracking().SingleAsync();
        saved.Score.Should().Be(score);
        saved.Passed.Should().Be(passed);
        saved.JobExamId.Should().Be(jobExam.Id);
        saved.CandidateId.Should().Be(request.CandidateId);
        var savedStage = await context.JobExams.AsNoTracking().SingleAsync();
        savedStage.ExamCandidateList.Should().Equal(otherCandidateId);

        var resubmit = () => handler.Handle(request, CancellationToken.None);
        await resubmit.Should().ThrowAsync<UnauthorizedException>();
        (await context.ExamAttempts.CountAsync()).Should().Be(1);
        Received.InOrder(() =>
        {
            _ = unitOfWork.BeginTransactionAsync(CancellationToken.None);
            _ = unitOfWork.SaveChangesAsync(CancellationToken.None);
            if (passed)
            {
                _ = publisher.PublishAsync(Arg.Is<ExamResultMessage>(message =>
                    message.UserId == request.CandidateId && message.JobId == jobExam.JobId), CancellationToken.None);
            }
            _ = unitOfWork.CommitTransactionAsync(CancellationToken.None);
        });
        await unitOfWork.DidNotReceive().RollbackTransactionAsync();
        await unitOfWork.Received(1).BeginTransactionAsync(CancellationToken.None);

        if (passed)
        {
            await publisher.ReceivedWithAnyArgs(1).PublishAsync(default!, default);
        }
        else
        {
            await publisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
        }
    }
}
