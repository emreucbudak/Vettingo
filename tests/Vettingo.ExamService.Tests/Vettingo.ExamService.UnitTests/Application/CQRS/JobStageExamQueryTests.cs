using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public class JobStageExamQueryTests
{
    [Fact]
    public async Task Handle_Should_Return_One_Exam_When_Multiple_Stages_Match()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.Company, Guid.NewGuid());
        var question = new MultipleChoiceQuestion();
        question.CreateQuestion(exam.Id, "Choose an answer", 2, 1);
        var option = new MultipleChoiceOption();
        option.CreateOption(question.Id, "Correct answer", true);
        question.AddOption(option);
        exam.AddMultipleChoiceQuestion(question);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1, 2);
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        var jobExam = new JobStageExam { Exam = exam };
        jobExam.CreateJobStageExam(Guid.NewGuid(), Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), [userId]);
        var attempt = new ExamAttempt();
        attempt.CreateAttempt(jobExam, Guid.NewGuid(), 80, DateTime.UtcNow, DateTime.UtcNow, passed: true);
        jobExam.AddAttempt(attempt);
        var otherExam = new Exam();
        otherExam.CreateExam("Other", 30, 50, ExamOwnerType.System, null);
        var otherJobExam = new JobStageExam { Exam = otherExam };
        otherJobExam.CreateJobStageExam(Guid.NewGuid(), jobExam.JobId, otherExam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), []);
        var matchingJobExam = new JobStageExam { Exam = exam };
        matchingJobExam.CreateJobStageExam(Guid.NewGuid(), jobExam.JobId, exam.Id, new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 11), [userId]);
        context.JobExams.AddRange(otherJobExam, jobExam, matchingJobExam);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = CreateHandler(context);

        var request = new GetJobStageExamByJobIdQueryRequest { JobId = jobExam.JobId, ExamId = exam.Id, UserId = userId };
        var response = await handler.Handle(request, CancellationToken.None);

        response.CompanyId.Should().Be(exam.CompanyId);
        response.OwnerType.Should().Be(exam.OwnerType);
        response.Title.Should().Be(exam.Title);
        response.DurationMinutes.Should().Be(exam.DurationMinutes);
        var returnedQuestion = response.MultipleChoiceQuestions.Should().ContainSingle().Subject;
        returnedQuestion.Id.Should().Be(question.Id);
        returnedQuestion.Options.Should().ContainSingle().Which.OptionText.Should().Be(option.OptionText);
        response.TrueFalseQuestions.Should().ContainSingle().Which.Id.Should().Be(trueFalseQuestion.Id);
        context.ChangeTracker.Entries<ExamAttempt>().Should().BeEmpty();
        var serialize = () => JsonSerializer.Serialize(response);
        serialize.Should().NotThrow();
        JsonSerializer.Serialize(response).Should().NotContain("\"Attempts\"").And.NotContain("\"IsCorrect\"");
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_In_Exam_Candidate_List_Should_Throw_UnauthorizedException()
    {
        await using var context = CreateContext();
        var exam = new Exam();
        exam.CreateExam("Backend", 60, 70, ExamOwnerType.Company, Guid.NewGuid());
        var jobExam = new JobStageExam { Exam = exam };
        jobExam.CreateJobStageExam(Guid.NewGuid(), Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), [Guid.NewGuid()]);
        context.JobExams.Add(jobExam);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = CreateHandler(context);

        var request = new GetJobStageExamByJobIdQueryRequest { JobId = jobExam.JobId, ExamId = exam.Id, UserId = Guid.NewGuid() };
        var action = () => handler.Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Bu sınava katılamazsınız");
    }

    [Fact]
    public async Task Handle_When_JobStageExam_Is_Missing_Should_Throw_NotFoundException()
    {
        await using var context = CreateContext();
        var handler = CreateHandler(context);

        var request = new GetJobStageExamByJobIdQueryRequest { JobId = Guid.NewGuid(), ExamId = Guid.NewGuid() };
        var action = () => handler.Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("İş ilanında istenen sınav bulunamadı");
    }

    private static ExamDbContext CreateContext() => new(new DbContextOptionsBuilder<ExamDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static GetJobStageExamByJobIdQueryHandler CreateHandler(ExamDbContext context)
    {
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        unitOfWork.JobStageExamRepository.Returns(new ExamRepository<JobStageExam>(context));
        return new GetJobStageExamByJobIdQueryHandler(unitOfWork, NullLogger<GetJobStageExamByJobIdQueryHandler>.Instance);
    }
}
