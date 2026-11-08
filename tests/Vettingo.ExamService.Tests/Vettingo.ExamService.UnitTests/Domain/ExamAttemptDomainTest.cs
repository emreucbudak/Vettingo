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
            var jobExam = CreateJobExam();
            var attempt = new ExamAttempt();
            var startedAt = new DateTime(2026, 11, 8, 7, 0, 0, DateTimeKind.Utc);
            var completedAt = startedAt.AddMinutes(30);

            attempt.CreateAttempt(jobExam, Guid.NewGuid(), score, startedAt, completedAt);

            attempt.Status.Should().Be(expectedStatus);
            attempt.Score.Should().Be(score);
            attempt.StartedAt.Should().Be(startedAt);
            attempt.CompletedAt.Should().Be(completedAt);
        }

        [Fact]
        public void Create_ExamAttempt_With_Valid_Parameters()
        {
            var jobExam = CreateJobExam();
            var candidateId = Guid.NewGuid();
            var attempt = new ExamAttempt();

            attempt.CreateAttempt(jobExam, candidateId, 80, DateTime.UtcNow, DateTime.UtcNow);

            attempt.JobExamId.Should().Be(jobExam.Id);
            attempt.CandidateId.Should().Be(candidateId);
        }

        [Fact]
        public void Create_ExamAttempt_With_Empty_JobExamId_Should_Throw()
        {
            var attempt = new ExamAttempt();
            Action action = () => attempt.CreateAttempt(new JobExam(), Guid.NewGuid(), 80, DateTime.UtcNow, DateTime.UtcNow);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ExamAttempt_With_Invalid_Score_Should_Throw()
        {
            var jobExam = CreateJobExam();
            var attempt = new ExamAttempt();
            Action action = () => attempt.CreateAttempt(jobExam, Guid.NewGuid(), 101, DateTime.UtcNow, DateTime.UtcNow);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private static JobExam CreateJobExam()
        {
            var exam = new Exam();
            exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
            var jobExam = new JobExam { Exam = exam };
            jobExam.CreateJobExam(Guid.NewGuid(), exam.Id);
            return jobExam;
        }
    }
}
