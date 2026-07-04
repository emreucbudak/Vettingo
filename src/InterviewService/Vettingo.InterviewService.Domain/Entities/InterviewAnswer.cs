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
            CheckInterviewAnswerContent(userId, interviewExamId, answerDate);
            SetId();
            UserId = userId;
            InterviewExamId = interviewExamId;
            AnswerDate = answerDate;
            SetCreatedAt();
        }

        public void CheckInterviewAnswerContent(Guid userId, Guid interviewExamId, DateOnly answerDate)
        {
            CheckGuid(userId, nameof(userId));
            CheckGuid(interviewExamId, nameof(interviewExamId));

            if (answerDate == default)
            {
                throw new ArgumentException("Cevap tarihi geçersiz.", nameof(answerDate));
            }
        }

        private static void CheckGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }
    }
}
