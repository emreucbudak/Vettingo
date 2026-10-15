using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class BaseEntityDomainTest
    {
        [Fact]
        public void Creation_Should_Assign_Identity_And_Utc_Date_To_All_Entities()
        {
            var before = DateOnly.FromDateTime(DateTime.UtcNow);
            var exam = new Exam();
            exam.CreateExam("Backend", "C#", "Backend exam", 60, 70, ExamOwnerType.System, null, null);
            var question = new MultipleChoiceQuestion();
            question.CreateQuestion(exam.Id, "Which option?", 1m, 1, "Explanation");
            var option = new MultipleChoiceOption();
            option.CreateOption(question.Id, "A", true, 1);
            var trueFalseQuestion = new TrueFalseQuestion();
            trueFalseQuestion.CreateQuestion(exam.Id, "Is this true?", 1m, 2, "Explanation", true);
            var attempt = new ExamAttempt();
            attempt.StartAttempt(exam.Id, Guid.NewGuid());
            var multipleChoiceAnswer = new ExamAnswer();
            multipleChoiceAnswer.CreateMultipleChoiceAnswer(attempt.Id, question.Id, option.Id, "A", true);
            var trueFalseAnswer = new ExamAnswer();
            trueFalseAnswer.CreateTrueFalseAnswer(attempt.Id, trueFalseQuestion.Id, true, true);
            var after = DateOnly.FromDateTime(DateTime.UtcNow);

            BaseEntity[] entities = [exam, question, option, trueFalseQuestion, attempt, multipleChoiceAnswer, trueFalseAnswer];
            entities.Select(entity => entity.Id).Should().OnlyHaveUniqueItems();
            foreach (var entity in entities)
            {
                entity.Id.Should().NotBeEmpty();
                entity.Id.Version.Should().Be(7);
                entity.CreatedAt.Should().BeOneOf(before, after);
            }
        }

        [Fact]
        public void Persistence_Should_Keep_Separate_Entities_With_Date_Columns()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
                .UseNpgsql("Host=localhost;Database=exam_model_test;Username=test;Password=test")
                .Options;
            using var context = new ExamDbContext(options);
            var entityTypes = typeof(BaseEntity).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(BaseEntity).IsAssignableFrom(type));

            foreach (var entityType in entityTypes)
            {
                var mapping = context.Model.FindEntityType(entityType);
                mapping.Should().NotBeNull();
                mapping!.BaseType.Should().BeNull();
                mapping.FindPrimaryKey()!.Properties.Should().ContainSingle(property => property.Name == nameof(BaseEntity.Id));
                var createdAt = mapping.FindProperty(nameof(BaseEntity.CreatedAt));
                createdAt.Should().NotBeNull();
                createdAt!.ClrType.Should().Be(typeof(DateOnly));
                createdAt.GetColumnType().Should().Be("date");
            }
        }
    }
}
