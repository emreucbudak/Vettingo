using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class JobInterview : BaseEntity
    {
        private List<Interview> _interviews = new();

        public Guid JobId { get; set; }
        public DateOnly StartedDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly WindowStartTime { get; set; }
        public TimeOnly WindowEndTime { get; set; }
        public List<Guid> CandidatesToInterview { get; set; } = new();
        public IReadOnlyCollection<Interview> Interviews => _interviews.AsReadOnly();

        public void CreateJobInterview(
            Guid stepId,
            Guid jobId,
            DateOnly startedDate,
            DateOnly endDate,
            TimeOnly windowStartTime,
            TimeOnly windowEndTime,
            IEnumerable<Guid> candidatesToInterview)
        {
            CandidatesToInterview = candidatesToInterview.ToList();
            Id = stepId;
            SetCreatedAt();
            JobId = jobId;
            StartedDate = startedDate;
            EndDate = endDate;
            WindowStartTime = windowStartTime;
            WindowEndTime = windowEndTime;
        }

        public void SetInterviews(List<Interview> interviews)
        {
            ArgumentNullException.ThrowIfNull(interviews);
            _interviews = interviews;
        }
    }
}
