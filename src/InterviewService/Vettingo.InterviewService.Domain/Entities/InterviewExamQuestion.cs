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
            SetId();
            InterviewExamId = interviewExamId;
            InterviewQuestionId = interviewQuestionId;
            DisplayOrder = displayOrder;
            SetCreatedAt();
        }
    }
}
