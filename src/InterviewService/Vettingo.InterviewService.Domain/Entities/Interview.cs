using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public TimeOnly StartedTime { get; private set; }
        public DateOnly InterviewDate { get; private set; }
        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set; }
        public Guid JobInterviewId { get; private set; }
        public JobInterview JobInterview { get; private set; } = null!;
        public string Chapter { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;
        public string MeetingLink { get; private set; } = string.Empty;
    }
}
