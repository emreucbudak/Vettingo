using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ExamAttemptDomainTest
    {
        [Theory]
        [InlineData(69, false)]
        [InlineData(70, true)]
        [InlineData(71, true)]
        public void Create_ExamAttempt_Should_Use_Exam_PassingScore(int score, bool passed)
        {
            var jobExam = CreateJobStageExam();
            var attempt = new ExamAttempt();
            var startedAt = new DateTime(2026, 11, 8, 7, 0, 0, DateTimeKind.Utc);
            var completedAt = startedAt.AddMinutes(30);

            attempt.CreateAttempt(jobExam, Guid.NewGuid(), score, startedAt, completedAt);

            attempt.Passed.Should().Be(passed);
            attempt.Score.Should().Be(score);
            attempt.StartedAt.Should().Be(startedAt);
            attempt.CompletedAt.Should().Be(completedAt);
        }

        [Fact]
        public void Create_ExamAttempt_With_Valid_Parameters()
        {
            var jobExam = CreateJobStageExam();
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
            var jobExam = new JobStageExam();
            Action action = () => attempt.CreateAttempt(jobExam, Guid.NewGuid(), 80, DateTime.UtcNow, DateTime.UtcNow);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ExamAttempt_With_Invalid_Score_Should_Throw()
        {
            var jobExam = CreateJobStageExam();
            var attempt = new ExamAttempt();
            Action action = () => attempt.CreateAttempt(jobExam, Guid.NewGuid(), 101, DateTime.UtcNow, DateTime.UtcNow);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddAttempt_Should_Remove_Only_The_Attempt_Candidate()
        {
            var jobExam = CreateJobStageExam();
            var candidateId = Guid.NewGuid();
            var otherCandidateId = Guid.NewGuid();
            jobExam.ExamCandidateList.AddRange([candidateId, otherCandidateId]);
            var attempt = new ExamAttempt();
            attempt.CreateAttempt(jobExam, candidateId, 80, DateTime.UtcNow, DateTime.UtcNow);

            jobExam.AddAttempt(attempt);

            jobExam.Attempts.Should().ContainSingle().Which.Should().BeSameAs(attempt);
            jobExam.ExamCandidateList.Should().Equal(otherCandidateId);
        }

        private static JobStageExam CreateJobStageExam()
        {
            var exam = new Exam();
            exam.CreateExam("Backend", "Backend exam", 60, 70, ExamOwnerType.System, null);
            var jobExam = new JobStageExam { Exam = exam };
            jobExam.CreateJobStageExam(Guid.NewGuid(), Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), []);
            return jobExam;
        }
    }
}
