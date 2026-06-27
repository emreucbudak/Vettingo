using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewAnswer : BaseEntity
    {
        public InterviewAnswer()
        {
        }

        public Guid UserId { get; private set; }
        public Guid InterviewExamId { get; private set; }
        public DateOnly AnswerDate { get; private set; }
        public InterviewExam? InterviewExam { get; private set; }

        public void CreateAnswer(Guid userId, Guid interviewExamId, DateOnly answerDate)
        {
            SetId();
            UserId = userId;
            InterviewExamId = interviewExamId;
            AnswerDate = answerDate;
            SetCreatedAt();
        }
    }
}
