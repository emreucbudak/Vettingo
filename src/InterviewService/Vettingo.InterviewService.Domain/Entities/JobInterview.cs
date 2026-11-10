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

        public void CreateJobInterview(Guid jobId, DateOnly startedDate, DateOnly endDate)
        {
            SetId();
            SetCreatedAt();
            JobId = jobId;
            StartedDate = startedDate;
            EndDate = endDate;
        }

        public void AddInterview(Interview interview)
        {
            ArgumentNullException.ThrowIfNull(interview);
            _interviews.Add(interview);
        }
    }
}
