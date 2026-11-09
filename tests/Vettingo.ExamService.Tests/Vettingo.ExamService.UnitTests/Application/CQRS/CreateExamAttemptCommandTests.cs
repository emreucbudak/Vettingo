using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;
using Vettingo.ExamService.Application.Messaging;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public sealed class CreateExamAttemptCommandTests
{
    [Theory]
    [InlineData(69, false)]
    [InlineData(70, true)]
    public async Task Should_Save_Attempt_And_Publish_Only_Passed_Results(int score, bool passed)
    {
        await using var context = new ExamDbContext(new DbContextOptionsBuilder<ExamDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var exam = new Exam();
        exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
        var jobExam = new JobExam { Exam = exam };
        jobExam.CreateJobExam(Guid.NewGuid(), exam.Id);
        context.JobExams.Add(jobExam);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var publisher = Substitute.For<IExamResultPublisher>();
        var handler = new CreateExamAttemptCommandHandler(
            new ExamRepository<JobExam>(context), new ExamRepository<ExamAttempt>(context), publisher);
        var completedAt = DateTime.UtcNow;
        var request = new CreateExamAttemptCommandRequest
        {
            JobExamId = jobExam.Id,
            CandidateId = Guid.NewGuid(),
            Score = score,
            StartedAt = completedAt.AddMinutes(-20),
            CompletedAt = completedAt
        };

        await handler.Handle(request, CancellationToken.None);

        var saved = await context.ExamAttempts.AsNoTracking().SingleAsync();
        saved.Passed.Should().Be(passed);
        saved.JobExamId.Should().Be(jobExam.Id);
        saved.CandidateId.Should().Be(request.CandidateId);
        if (passed)
        {
            await publisher.Received(1).PublishAsync(Arg.Is<ExamResultMessage>(message =>
                message.UserId == request.CandidateId && message.JobId == jobExam.JobId), CancellationToken.None);
        }
        else
        {
            await publisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
        }
    }
}
