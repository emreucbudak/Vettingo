using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class JobInterview : BaseEntity
    {
        private readonly List<Interview> _interviews = new();

        public Guid JobId { get; set; }
        public DateOnly StartedDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly WindowStartTime { get; set; }
        public TimeOnly WindowEndTime { get; set; }
        public List<Guid> CandidatesToInterview { get; set; } = new();
        public IReadOnlyCollection<Interview> Interviews => _interviews.AsReadOnly();

        public void CreateJobInterview(
            Guid jobId,
            DateOnly startedDate,
            DateOnly endDate,
            TimeOnly windowStartTime,
            TimeOnly windowEndTime,
            IEnumerable<Guid> candidatesToInterview)
        {
            CandidatesToInterview = candidatesToInterview.ToList();
            SetId();
            SetCreatedAt();
            JobId = jobId;
            StartedDate = startedDate;
            EndDate = endDate;
            WindowStartTime = windowStartTime;
            WindowEndTime = windowEndTime;
        }

        public void AddInterview(Interview interview)
        {
            ArgumentNullException.ThrowIfNull(interview);
            _interviews.Add(interview);
        }
    }
}
