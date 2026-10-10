using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public TimeOnly StartedTime { get; private set; }
        public DateOnly InterviewDate { get; private set; }
        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Surname { get; private set; } = string.Empty;
        public string Chapter { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;
        public string WhereIsMeeting { get; private set; } = string.Empty;
        public string? MeetingLink { get; private set; } = string.Empty;
    }
}
