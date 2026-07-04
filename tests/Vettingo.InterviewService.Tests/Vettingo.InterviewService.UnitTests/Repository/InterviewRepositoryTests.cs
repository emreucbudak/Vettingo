using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Domain.Entities;
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
            exam.CreateExam(Guid.NewGuid(), "Backend Interview", "Backend interview description", [questionId]);

            await repository.AddAsync(exam);
            await repository.SaveChangesAsync();

            var result = await repository.GetByIdAsync(exam.Id, "Questions");

            result.Should().NotBeNull();
            result!.Questions.Should().ContainSingle();
            result.Questions.Single().InterviewQuestionId.Should().Be(questionId);
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
    }
}
