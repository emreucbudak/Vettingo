using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    public class ExamAttempt
    {
        public ExamAttempt()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public Guid CandidateId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public decimal Score { get; private set; }
        public ExamAttemptStatus Status { get; private set; }
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void StartAttempt(Guid examId, Guid candidateId)
        {
            SetId();
            ExamId = examId;
            CandidateId = candidateId;
            StartedAt = DateTime.UtcNow;
            Status = ExamAttemptStatus.Started;
        }

        public void CompleteAttempt(decimal score)
        {
            Score = score;
            CompletedAt = DateTime.UtcNow;
            Status = ExamAttemptStatus.Completed;
        }

        public void EvaluateAttempt(decimal score)
        {
            Score = score;
            Status = ExamAttemptStatus.Evaluated;
        }

        public void CancelAttempt()
        {
            Status = ExamAttemptStatus.Cancelled;
        }
    }
}
