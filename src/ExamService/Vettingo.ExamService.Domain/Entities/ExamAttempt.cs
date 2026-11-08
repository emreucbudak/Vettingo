using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    public class ExamAttempt : BaseEntity
    {
        public ExamAttempt()
        {
        }

        public Guid JobExamId { get; private set; }
        public JobExam JobExam { get; private set; } = null!;
        public Guid CandidateId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public int Score { get; private set; }
        public ExamAttemptStatus Status { get; private set; }

        public void CreateAttempt(JobExam jobExam, Guid candidateId, int score, DateTime startedAt, DateTime completedAt)
        {
            ArgumentNullException.ThrowIfNull(jobExam);
            CheckGuid(jobExam.Id, nameof(JobExamId));
            CheckGuid(candidateId, nameof(candidateId));
            CheckScore(score);
            var exam = jobExam.Exam ?? throw new ArgumentException("İlana bağlı sınav yüklenmelidir.", nameof(jobExam));

            SetId();
            SetCreatedAt();
            JobExam = jobExam;
            JobExamId = jobExam.Id;
            CandidateId = candidateId;
            StartedAt = startedAt.ToUniversalTime();
            Score = score;
            CompletedAt = completedAt.ToUniversalTime();
            Status = score >= exam.PassingScore ? ExamAttemptStatus.Passed : ExamAttemptStatus.Failed;
        }

        public void CheckScore(int score)
        {
            if (score is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(score), score, "Skor 0 ile 100 arasında olmalıdır.");
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
