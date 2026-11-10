using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public class JobStageExamQueryTests
{
    [Fact]
    public async Task Handle_Should_Find_By_JobId_And_Return_Exam_With_Related_Data()
    {
        await using var context = CreateContext();
        var exam = new Exam();
        exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.Company, Guid.NewGuid());
        var question = new MultipleChoiceQuestion();
        question.CreateQuestion(exam.Id, "Choose an answer", 2, 1);
        var option = new MultipleChoiceOption();
        option.CreateOption(question.Id, "Correct answer", true);
        question.AddOption(option);
        exam.AddMultipleChoiceQuestion(question);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2, "True", "False");
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        var jobExam = new JobStageExam { Exam = exam };
        jobExam.CreateJobStageExam(Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), []);
        context.Entry(jobExam).Property(entity => entity.Id).CurrentValue = Guid.NewGuid();
        var attempt = new ExamAttempt();
        attempt.CreateAttempt(jobExam, Guid.NewGuid(), 80, DateTime.UtcNow, DateTime.UtcNow);
        jobExam.AddAttempt(attempt);
        var otherExam = new Exam();
        otherExam.CreateExam("Other", "Other exam", 30, 50, ExamOwnerType.System, null);
        var otherJobExam = new JobStageExam { Exam = otherExam };
        otherJobExam.CreateJobStageExam(Guid.NewGuid(), otherExam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), []);
        context.JobExams.AddRange(otherJobExam, jobExam);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = CreateHandler(context);

        var response = await handler.Handle(new GetJobStageExamByJobIdQueryRequest { JobId = jobExam.JobId }, CancellationToken.None);

        response.CompanyId.Should().Be(exam.CompanyId);
        response.OwnerType.Should().Be(exam.OwnerType);
        response.Title.Should().Be(exam.Title);
        response.Description.Should().Be(exam.Description);
        response.DurationMinutes.Should().Be(exam.DurationMinutes);
        var returnedQuestion = response.MultipleChoiceQuestions.Should().ContainSingle().Subject;
        returnedQuestion.Id.Should().Be(question.Id);
        returnedQuestion.Options.Should().ContainSingle().Which.OptionText.Should().Be(option.OptionText);
        response.TrueFalseQuestions.Should().ContainSingle().Which.TrueAnswer.Should().Be(trueFalseQuestion.TrueAnswer);
        context.ChangeTracker.Entries<ExamAttempt>().Should().BeEmpty();
        var serialize = () => JsonSerializer.Serialize(response);
        serialize.Should().NotThrow();
        JsonSerializer.Serialize(response).Should().NotContain("\"Attempts\"");
    }

    [Fact]
    public async Task Handle_When_JobStageExam_Is_Missing_Should_Throw_NotFoundException()
    {
        await using var context = CreateContext();
        var handler = CreateHandler(context);

        var action = () => handler.Handle(new GetJobStageExamByJobIdQueryRequest { JobId = Guid.NewGuid() }, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    private static ExamDbContext CreateContext() => new(new DbContextOptionsBuilder<ExamDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static GetJobStageExamByJobIdQueryHandler CreateHandler(ExamDbContext context) => new(
        new ExamRepository<JobStageExam>(context), new ExamRepository<Exam>(context), NullLogger<GetJobStageExamByJobIdQueryHandler>.Instance);
}
