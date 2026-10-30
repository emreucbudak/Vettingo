using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewResult : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid InterviewId { get; private set; }
        public int InterviewScore { get; private set; }
        public bool Passed { get; private set; }

        public void Create(Guid userId, Guid interviewId, int interviewScore, bool passed)
        {
            SetId();
            SetCreatedAt();
            UserId = userId;
            InterviewId = interviewId;
            InterviewScore = interviewScore;
            Passed = passed;
        }
    }
}
