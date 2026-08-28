using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.UnitTests.Repository
{
    public class ExamRepositoryTests
    {
        [Fact]
        public async Task AddExamAsync_Then_GetExamByIdAsync_Should_Return_Exam()
        {
            await using var context = CreateContext();
            var repository = new ExamRepository(context);
            var exam = CreateExam("Backend Exam");

            await repository.AddExamAsync(exam);
            await repository.SaveChangesAsync();

            var result = await repository.GetExamByIdAsync(exam.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(exam.Id);
            result.Title.Should().Be("Backend Exam");
        }

        [Fact]
        public async Task QuestionRepository_Should_Return_MultipleChoiceQuestions_With_Options_By_ExamId()
        {
            await using var context = CreateContext();
            var repository = new QuestionRepository(context);
            var examId = Guid.NewGuid();
            var firstQuestion = CreateMultipleChoiceQuestion(examId, "First question", 2);
            var secondQuestion = CreateMultipleChoiceQuestion(examId, "Second question", 1);
            var otherExamQuestion = CreateMultipleChoiceQuestion(Guid.NewGuid(), "Other exam question", 3);

            await repository.AddMultipleChoiceQuestionAsync(firstQuestion);
            await repository.AddMultipleChoiceQuestionAsync(secondQuestion);
            await repository.AddMultipleChoiceQuestionAsync(otherExamQuestion);
            await repository.SaveChangesAsync();

            var result = (await repository.GetMultipleChoiceQuestionsByExamIdAsync(examId)).ToList();

            result.Should().HaveCount(2);
            result.Select(question => question.DisplayOrder).Should().Equal(1, 2);
            result.Should().OnlyContain(question => question.ExamId == examId);
            result[0].Options.Should().HaveCount(2);
        }

        private static ExamDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
                .UseInMemoryDatabase($"exam-repository-{Guid.NewGuid()}")
                .Options;

            return new ExamDbContext(options);
        }

        private static Exam CreateExam(string title)
        {
            Exam exam = new();
            exam.CreateExam(title, "Backend", "Backend exam description", 60, 70, ExamOwnerType.System, null, null);
            return exam;
        }

        private static MultipleChoiceQuestion CreateMultipleChoiceQuestion(Guid examId, string questionText, int displayOrder)
        {
            MultipleChoiceQuestion question = new();
            question.CreateQuestion(examId, questionText, 1m, displayOrder, "Explanation");

            MultipleChoiceOption firstOption = new();
            firstOption.CreateOption(question.Id, "A", true, 2);
            MultipleChoiceOption secondOption = new();
            secondOption.CreateOption(question.Id, "B", false, 1);
            question.AddOption(firstOption);
            question.AddOption(secondOption);

            return question;
        }
    }
}
