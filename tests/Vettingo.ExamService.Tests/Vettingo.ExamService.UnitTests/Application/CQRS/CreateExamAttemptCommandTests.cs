using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.ExamService.Application.DTOs;
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
    [Theory]
    [InlineData(69, false, false)]
    [InlineData(70, true, false)]
    [InlineData(70, true, true)]
    public async Task Should_Save_Attempt_And_Publish_Only_Passed_Results(int score, bool passed, bool publishFails)
    {
        await using var context = new ExamDbContext(new DbContextOptionsBuilder<ExamDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var exam = new Exam();
        exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
        var candidateId = Guid.NewGuid();
        var otherCandidateId = Guid.NewGuid();
        var jobExam = new JobStageExam { Exam = exam };
        jobExam.CreateJobStageExam(Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9),
            [candidateId, otherCandidateId]);
        context.JobExams.Add(jobExam);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var publisher = Substitute.For<IExamResultPublisher>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        unitOfWork.JobStageExamRepository.Returns(new ExamRepository<JobStageExam>(context));
        unitOfWork.ExamAttemptRepository.Returns(new ExamRepository<ExamAttempt>(context));
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(call => context.SaveChangesAsync(call.Arg<CancellationToken>()));
        var handler = new CreateExamAttemptCommandHandler(unitOfWork, publisher);
        var completedAt = DateTime.UtcNow;
        var request = new CreateExamAttemptCommandRequest
        {
            JobStageExamId = jobExam.Id,
            CandidateId = candidateId,
            Score = score,
            StartedAt = completedAt.AddMinutes(-20),
            CompletedAt = completedAt
        };

        await handler.Handle(request with { CandidateId = Guid.NewGuid() }, CancellationToken.None);
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
        saved.Passed.Should().Be(passed);
        saved.JobExamId.Should().Be(jobExam.Id);
        saved.CandidateId.Should().Be(request.CandidateId);
        var savedStage = await context.JobExams.AsNoTracking().SingleAsync();
        savedStage.ExamCandidateList.Should().Equal(otherCandidateId);

        await handler.Handle(request, CancellationToken.None);
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
