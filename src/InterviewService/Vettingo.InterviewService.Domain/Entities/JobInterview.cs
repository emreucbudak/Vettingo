using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class JobInterview : BaseEntity
    {
        public Guid JobId { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime EndDate { get; set; }

        public void CreateJobInterview(Guid jobId, DateTime startedDate, DateTime endDate)
        {
            SetId();
            SetCreatedAt();
            JobId = jobId;
            StartedDate = startedDate;
            EndDate = endDate;
        }
    }
}
