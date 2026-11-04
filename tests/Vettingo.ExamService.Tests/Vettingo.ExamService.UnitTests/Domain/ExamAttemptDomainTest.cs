using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ExamAttemptDomainTest
    {
        [Fact]
        public void ExamAttempt_Should_Remain_NotAttempted_Until_Completed()
        {
            var attempt = new ExamAttempt();
            attempt.Status.Should().Be(ExamAttemptStatus.NotAttempted);
            attempt.StartAttempt(Guid.NewGuid(), Guid.NewGuid());
            attempt.Status.Should().Be(ExamAttemptStatus.NotAttempted);
        }

        [Theory]
        [InlineData(69, ExamAttemptStatus.Failed)]
        [InlineData(70, ExamAttemptStatus.Passed)]
        [InlineData(71, ExamAttemptStatus.Passed)]
        public void Complete_ExamAttempt_Should_Use_Exam_PassingScore(int score, ExamAttemptStatus expectedStatus)
        {
            using var context = new ExamDbContext(new DbContextOptionsBuilder<ExamDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var exam = new Exam();
            exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
            var attempt = new ExamAttempt();
            attempt.StartAttempt(exam.Id, Guid.NewGuid());
            context.Exams.Add(exam);
            context.ExamAttempts.Add(attempt);

            attempt.CompleteAttempt(score);

            attempt.Status.Should().Be(expectedStatus);
            attempt.Score.Should().Be(score);
            attempt.CompletedAt.Should().NotBeNull();
        }

        [Fact]
        public void Start_ExamAttempt_With_Valid_Parameters()
        {
            // Arrange
            var examId = Guid.NewGuid();
            var candidateId = Guid.NewGuid();

            // Act
            Action action = () =>
            {
                var attempt = new ExamAttempt();
                attempt.StartAttempt(examId, candidateId);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Start_ExamAttempt_With_Empty_ExamId_Should_Throw()
        {
            // Arrange
            var candidateId = Guid.NewGuid();

            // Act
            Action action = () =>
            {
                var attempt = new ExamAttempt();
                attempt.StartAttempt(Guid.Empty, candidateId);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Complete_ExamAttempt_With_Invalid_Score_Should_Throw()
        {
            // Arrange
            var attempt = new ExamAttempt();
            attempt.StartAttempt(Guid.NewGuid(), Guid.NewGuid());

            // Act
            Action action = () => attempt.CompleteAttempt(101);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
