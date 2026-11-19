using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class ExamAttempt : BaseEntity
    {
        public ExamAttempt()
        {
        }

        public Guid JobExamId { get; private set; }
        public JobStageExam JobExam { get; private set; } = null!;
        public Guid CandidateId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public int Score { get; private set; }
        public bool Passed { get; private set; }

        public void CreateAttempt(
            JobStageExam jobExam, Guid candidateId, int score, DateTime startedAt, DateTime completedAt, bool passed)
        {
            ArgumentNullException.ThrowIfNull(jobExam);
            CheckGuid(jobExam.Id, nameof(JobExamId));
            CheckGuid(candidateId, nameof(candidateId));
            CheckScore(score);

            SetId();
            SetCreatedAt();
            JobExam = jobExam;
            JobExamId = jobExam.Id;
            CandidateId = candidateId;
            StartedAt = startedAt.ToUniversalTime();
            Score = score;
            CompletedAt = completedAt.ToUniversalTime();
            Passed = passed;
        }

        public void CheckScore(int score)
        {
            if (score < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(score), score, "Skor 0'dan küçük olamaz.");
            }
        }

        private static void CheckGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }
    }
}
