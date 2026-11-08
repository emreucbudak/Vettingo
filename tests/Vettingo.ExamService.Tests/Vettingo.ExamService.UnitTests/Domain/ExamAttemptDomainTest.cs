using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ExamAttemptDomainTest
    {
        [Theory]
        [InlineData(69, ExamAttemptStatus.Failed)]
        [InlineData(70, ExamAttemptStatus.Passed)]
        [InlineData(71, ExamAttemptStatus.Passed)]
        public void Create_ExamAttempt_Should_Use_Exam_PassingScore(int score, ExamAttemptStatus expectedStatus)
        {
            var exam = new Exam();
            exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
            var attempt = new ExamAttempt();
            var startedAt = new DateTime(2026, 11, 8, 7, 0, 0, DateTimeKind.Utc);
            var completedAt = startedAt.AddMinutes(30);

            attempt.CreateAttempt(exam, Guid.NewGuid(), score, startedAt, completedAt);

            attempt.Status.Should().Be(expectedStatus);
            attempt.Score.Should().Be(score);
            attempt.StartedAt.Should().Be(startedAt);
            attempt.CompletedAt.Should().Be(completedAt);
        }

        [Fact]
        public void Create_ExamAttempt_With_Valid_Parameters()
        {
            var exam = new Exam();
            exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
            var candidateId = Guid.NewGuid();
            var attempt = new ExamAttempt();

            attempt.CreateAttempt(exam, candidateId, 80, DateTime.UtcNow, DateTime.UtcNow);

            attempt.ExamId.Should().Be(exam.Id);
            attempt.CandidateId.Should().Be(candidateId);
        }

        [Fact]
        public void Create_ExamAttempt_With_Empty_ExamId_Should_Throw()
        {
            var attempt = new ExamAttempt();
            Action action = () => attempt.CreateAttempt(new Exam(), Guid.NewGuid(), 80, DateTime.UtcNow, DateTime.UtcNow);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ExamAttempt_With_Invalid_Score_Should_Throw()
        {
            var exam = new Exam();
            exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
            var attempt = new ExamAttempt();
            Action action = () => attempt.CreateAttempt(exam, Guid.NewGuid(), 101, DateTime.UtcNow, DateTime.UtcNow);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
