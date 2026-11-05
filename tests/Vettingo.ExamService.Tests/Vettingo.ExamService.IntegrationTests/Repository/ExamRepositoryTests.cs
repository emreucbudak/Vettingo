using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.IntegrationTests;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.UnitTests.Repository
{
    public class ExamRepositoryTests : IClassFixture<PostgreSqlContainerFixture>
    {
        private readonly PostgreSqlContainerFixture _fixture;

        public ExamRepositoryTests(PostgreSqlContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task AddAsync_Then_GetExamByIdAsync_Should_Return_Exam()
        {
            await using ExamDbContext context = _fixture.CreateDbContext();
            var repository = new ExamRepository<Exam>(context);
            var exam = CreateExam("Backend Exam");

            await repository.AddAsync(exam);
            await repository.SaveChangesAsync();

            var result = await repository.GetByIdAsync(exam.Id, "MultipleChoiceQuestions.Options", "TrueFalseQuestions");

            result.Should().NotBeNull();
            result!.Id.Should().Be(exam.Id);
            result.Title.Should().Be("Backend Exam");
        }

        [Fact]
        public async Task ExamRepository_Should_Return_Exam_With_Questions_And_Options()
        {
            await using ExamDbContext context = _fixture.CreateDbContext();
            var repository = new ExamRepository<Exam>(context);
            var exam = CreateExam("Question exam");
            var otherExam = CreateExam("Other exam");
            var firstQuestion = CreateMultipleChoiceQuestion(exam.Id, "First question", 2);
            var secondQuestion = CreateMultipleChoiceQuestion(exam.Id, "Second question", 1);
            var otherExamQuestion = CreateMultipleChoiceQuestion(otherExam.Id, "Other exam question", 3);

            exam.AddMultipleChoiceQuestion(firstQuestion);
            exam.AddMultipleChoiceQuestion(secondQuestion);
            otherExam.AddMultipleChoiceQuestion(otherExamQuestion);
            await repository.AddAsync(exam);
            await repository.AddAsync(otherExam);
            await repository.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await repository.GetByIdAsync(exam.Id, "MultipleChoiceQuestions.Options", "TrueFalseQuestions");

            result.Should().NotBeNull();
            result!.MultipleChoiceQuestions.Should().HaveCount(2);
            result.MultipleChoiceQuestions.Should().OnlyContain(question => question.ExamId == exam.Id);
            result.MultipleChoiceQuestions.Should().OnlyContain(question => question.Options.Count == 2);
        }

        private static Exam CreateExam(string title)
        {
            Exam exam = new();
            exam.CreateExam(title, "Backend exam description", 60, 70, ExamOwnerType.System, null);
            return exam;
        }

        private static MultipleChoiceQuestion CreateMultipleChoiceQuestion(Guid examId, string questionText, int displayOrder)
        {
            MultipleChoiceQuestion question = new();
            question.CreateQuestion(examId, questionText, 1, displayOrder);

            MultipleChoiceOption firstOption = new();
            firstOption.CreateOption(question.Id, "A", true);
            MultipleChoiceOption secondOption = new();
            secondOption.CreateOption(question.Id, "B", false);
            question.AddOption(firstOption);
            question.AddOption(secondOption);

            return question;
        }
    }
}
