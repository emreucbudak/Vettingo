using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewQuestion : BaseEntity
    {
        public InterviewQuestion()
        {
        }

        public Guid? CompanyId { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public ICollection<InterviewExamQuestion> ExamQuestions { get; private set; } = new List<InterviewExamQuestion>();

        public void CreateQuestion(Guid? companyId, string questionText)
        {
            CheckInterviewQuestionContent(companyId, questionText);
            SetId();
            CompanyId = companyId;
            QuestionText = questionText;
            SetCreatedAt();
            UpdatedAt = null;
        }

        public void UpdateQuestion(Guid? companyId, string questionText)
        {
            CheckInterviewQuestionContent(companyId, questionText);
            CompanyId = companyId;
            QuestionText = questionText;
            SetUpdatedAt();
        }

        public void CheckInterviewQuestionContent(Guid? companyId, string questionText)
        {
            if (companyId.HasValue && companyId.Value == Guid.Empty)
            {
                throw new ArgumentException("CompanyId boş olamaz.", nameof(companyId));
            }

            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionText, nameof(questionText));
        }
    }
}
