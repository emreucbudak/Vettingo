using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ExamAttemptDomainTest
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(69, true)]
        [InlineData(70, false)]
        [InlineData(71, true)]
        [InlineData(101, true)]
        public void Create_ExamAttempt_Should_Store_The_Provided_Result(int score, bool passed)
        {
            var jobExam = CreateJobStageExam();
            var attempt = new ExamAttempt();
            var startedAt = new DateTime(2026, 11, 8, 7, 0, 0, DateTimeKind.Utc);
            var completedAt = startedAt.AddMinutes(30);

            attempt.CreateAttempt(jobExam, Guid.NewGuid(), score, startedAt, completedAt, passed);

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

            attempt.CreateAttempt(jobExam, candidateId, 80, DateTime.UtcNow, DateTime.UtcNow, passed: true);

            attempt.JobExamId.Should().Be(jobExam.Id);
            attempt.CandidateId.Should().Be(candidateId);
        }

        [Fact]
        public void Create_ExamAttempt_With_Empty_JobExamId_Should_Throw()
        {
            var attempt = new ExamAttempt();
            var jobExam = new JobStageExam();
            Action action = () => attempt.CreateAttempt(jobExam, Guid.NewGuid(), 80, DateTime.UtcNow, DateTime.UtcNow, passed: true);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ExamAttempt_With_Negative_Score_Should_Throw()
        {
            var jobExam = CreateJobStageExam();
            var attempt = new ExamAttempt();
            Action action = () => attempt.CreateAttempt(jobExam, Guid.NewGuid(), -1, DateTime.UtcNow, DateTime.UtcNow, passed: false);

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
            attempt.CreateAttempt(jobExam, candidateId, 80, DateTime.UtcNow, DateTime.UtcNow, passed: true);

            jobExam.AddAttempt(attempt);

            jobExam.Attempts.Should().ContainSingle().Which.Should().BeSameAs(attempt);
            jobExam.ExamCandidateList.Should().Equal(otherCandidateId);
        }

        private static JobStageExam CreateJobStageExam()
        {
            var exam = new Exam();
            exam.CreateExam("Backend", 60, 70, ExamOwnerType.System, null);
            var jobExam = new JobStageExam { Exam = exam };
            jobExam.CreateJobStageExam(Guid.NewGuid(), Guid.NewGuid(), exam.Id, new DateOnly(2026, 11, 8), new DateOnly(2026, 11, 9), []);
            return jobExam;
        }
    }
}
