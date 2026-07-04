using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewExamQuestion : BaseEntity
    {
        public InterviewExamQuestion()
        {
        }

        public Guid InterviewExamId { get; private set; }
        public Guid InterviewQuestionId { get; private set; }
        public int DisplayOrder { get; private set; }
        public InterviewExam? InterviewExam { get; private set; }
        public InterviewQuestion? InterviewQuestion { get; private set; }

        public void CreateExamQuestion(Guid interviewExamId, Guid interviewQuestionId, int displayOrder)
        {
            CheckInterviewExamQuestionContent(interviewExamId, interviewQuestionId, displayOrder);
            SetId();
            InterviewExamId = interviewExamId;
            InterviewQuestionId = interviewQuestionId;
            DisplayOrder = displayOrder;
            SetCreatedAt();
        }

        public void CheckInterviewExamQuestionContent(Guid interviewExamId, Guid interviewQuestionId, int displayOrder)
        {
            CheckGuid(interviewExamId, nameof(interviewExamId));
            CheckGuid(interviewQuestionId, nameof(interviewQuestionId));

            if (displayOrder <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(displayOrder), displayOrder, "Gösterim sırası sıfırdan büyük olmalıdır.");
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
