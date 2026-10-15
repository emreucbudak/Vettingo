using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    public class ExamAttempt : BaseEntity
    {
        public ExamAttempt()
        {
        }

        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public Guid CandidateId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public decimal Score { get; private set; }
        public ExamAttemptStatus Status { get; private set; }
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void StartAttempt(Guid examId, Guid candidateId)
        {
            CheckStartAttemptContent(examId, candidateId);
            SetId();
            SetCreatedAt();
            ExamId = examId;
            CandidateId = candidateId;
            StartedAt = DateTime.UtcNow;
            Status = ExamAttemptStatus.Started;
        }

        public void CompleteAttempt(decimal score)
        {
            CheckScore(score);
            Score = score;
            CompletedAt = DateTime.UtcNow;
            Status = ExamAttemptStatus.Completed;
        }

        public void EvaluateAttempt(decimal score)
        {
            CheckScore(score);
            Score = score;
            Status = ExamAttemptStatus.Evaluated;
        }

        public void CancelAttempt()
        {
            Status = ExamAttemptStatus.Cancelled;
        }

        public void CheckStartAttemptContent(Guid examId, Guid candidateId)
        {
            CheckGuid(examId, nameof(examId));
            CheckGuid(candidateId, nameof(candidateId));
        }

        public void CheckScore(decimal score)
        {
            if (score is < 0m or > 100m)
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
