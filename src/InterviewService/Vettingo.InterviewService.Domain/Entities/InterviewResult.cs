using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewResult : BaseEntity
    {
        public Guid InterviewId { get; private set; }
        public Interview Interview { get; private set; } = null!;
        public int InterviewScore { get; private set; }
        public bool Passed { get; private set; }

        public void Create(Guid interviewId, int interviewScore, bool passed)
        {
            SetId();
            SetCreatedAt();
            InterviewId = interviewId;
            InterviewScore = interviewScore;
            Passed = passed;
        }

        public void Update(int? interviewScore, bool? passed)
        {
            if (interviewScore is null && passed is null)
            {
                return;
            }

            if (interviewScore.HasValue)
            {
                InterviewScore = interviewScore.Value;
            }

            if (passed.HasValue)
            {
                Passed = passed.Value;
            }

            SetUpdatedAt();
        }
    }
}
