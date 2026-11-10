using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class JobStageExam : BaseEntity
    {
        private readonly List<ExamAttempt> _attempts = new();

        public Guid JobId { get; set; }
        public Guid ExamId { get; set; }
        public DateOnly StartedDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<Guid> ExamCandidateList { get; set; } = new();
        public Exam Exam { get; set; } = null!;
        public IReadOnlyCollection<ExamAttempt> Attempts => _attempts.AsReadOnly();

        public void CreateJobStageExam(
            Guid jobId, Guid examId, DateOnly startedDate, DateOnly endDate, IEnumerable<Guid> examCandidateList)
        {
            ExamCandidateList = examCandidateList.ToList();
            SetCreatedAt();
            JobId = jobId;
            ExamId = examId;
            StartedDate = startedDate;
            EndDate = endDate;
        }

        public void AddAttempt(ExamAttempt attempt)
        {
            ArgumentNullException.ThrowIfNull(attempt, nameof(attempt));
            _attempts.Add(attempt);
            ExamCandidateList.Remove(attempt.CandidateId);
        }
    }
}
