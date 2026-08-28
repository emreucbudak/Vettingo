using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Domain.Enums;
using Vettingo.InterviewService.Persistence.DbContext;

namespace Vettingo.InterviewService.UnitTests.Repository
{
    public class InterviewRepositoryTests
    {
        [Fact]
        public async Task AddAsync_Then_GetByIdAsync_Should_Return_Entity()
        {
            await using var context = CreateContext();
            var repository = new Persistence.Repository.Repository<InterviewQuestion>(context);
            var question = CreateQuestion(Guid.NewGuid(), "Tell us about your last project.");

            await repository.AddAsync(question);
            await repository.SaveChangesAsync();

            var result = await repository.GetByIdAsync(question.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(question.Id);
            result.QuestionText.Should().Be(question.QuestionText);
        }

        [Fact]
        public async Task GetAllAsync_Should_Filter_Entities_With_Predicate()
        {
            await using var context = CreateContext();
            var repository = new Persistence.Repository.Repository<InterviewQuestion>(context);
            var companyId = Guid.NewGuid();
            var otherCompanyId = Guid.NewGuid();

            await repository.AddAsync(CreateQuestion(companyId, "Company question"));
            await repository.AddAsync(CreateQuestion(null, "Global question"));
            await repository.AddAsync(CreateQuestion(otherCompanyId, "Other company question"));
            await repository.SaveChangesAsync();

            var result = (await repository.GetAllAsync(question => question.CompanyId == null || question.CompanyId == companyId)).ToList();

            result.Should().HaveCount(2);
            result.Should().OnlyContain(question => question.CompanyId == null || question.CompanyId == companyId);
        }

        [Fact]
        public async Task GetByIdAsync_With_IncludeProperties_Should_Load_Navigation()
        {
            await using var context = CreateContext();
            var repository = new Persistence.Repository.Repository<InterviewExam>(context);
            var questionId = Guid.NewGuid();
            var exam = new InterviewExam();
            var startDate = DateTime.UtcNow.AddDays(1);
            exam.CreateExam(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Backend Interview",
                "Backend interview description",
                InterviewType.AI,
                startDate,
                startDate.AddHours(2),
                [questionId]);

            await repository.AddAsync(exam);
            await repository.SaveChangesAsync();

            var result = await repository.GetByIdAsync(exam.Id, "Questions");

            result.Should().NotBeNull();
            result!.Questions.Should().ContainSingle();
            result.Questions.Single().InterviewQuestionId.Should().Be(questionId);
        }

        [Fact]
        public async Task GetAllAsync_Should_Filter_Upcoming_Interviews_By_Candidate()
        {
            await using var context = CreateContext();
            var repository = new Persistence.Repository.Repository<InterviewExam>(context);
            var candidateId = Guid.NewGuid();
            var upcoming = CreateHumanInterview(candidateId, DateTime.UtcNow.AddDays(1));
            var past = CreateHumanInterview(candidateId, DateTime.UtcNow.AddDays(-1));
            var otherCandidate = CreateHumanInterview(Guid.NewGuid(), DateTime.UtcNow.AddHours(1));
            await repository.AddAsync(upcoming);
            await repository.AddAsync(past);
            await repository.AddAsync(otherCandidate);
            await repository.SaveChangesAsync();
            var now = DateTime.UtcNow;

            var result = (await repository.GetAllAsync(
                exam => exam.CandidateId == candidateId && exam.StartDate >= now,
                query => query.OrderBy(exam => exam.StartDate))).ToList();

            result.Should().ContainSingle();
            result.Single().Id.Should().Be(upcoming.Id);
        }

        private static InterviewDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<InterviewDbContext>()
                .UseInMemoryDatabase($"interview-repository-{Guid.NewGuid()}")
                .Options;

            return new InterviewDbContext(options);
        }

        private static InterviewQuestion CreateQuestion(Guid? companyId, string questionText)
        {
            InterviewQuestion question = new();
            question.CreateQuestion(companyId, questionText);
            return question;
        }

        private static InterviewExam CreateHumanInterview(Guid candidateId, DateTime startDate)
        {
            InterviewExam interview = new();
            interview.CreateExam(
                Guid.NewGuid(),
                candidateId,
                "Human Interview",
                "Human interview description",
                InterviewType.Human,
                startDate,
                null,
                []);
            return interview;
        }
    }
}
