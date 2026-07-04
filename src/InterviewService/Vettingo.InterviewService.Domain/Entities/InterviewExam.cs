using Vettingo.InterviewService.Domain.Common;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewExam : BaseEntity
    {
        public InterviewExam()
        {
        }

        public Guid CompanyId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public ICollection<InterviewExamQuestion> Questions { get; private set; } = new List<InterviewExamQuestion>();
        public ICollection<InterviewAnswer> Answers { get; private set; } = new List<InterviewAnswer>();

        public void CreateExam(Guid companyId, string title, string description, IEnumerable<Guid> questionIds)
        {
            CheckInterviewExamContent(companyId, title, description, questionIds);
            SetId();
            CompanyId = companyId;
            Title = title;
            Description = description;
            SetCreatedAt();
            UpdatedAt = null;
            SetQuestions(questionIds);
        }

        public void UpdateExam(string title, string description, IEnumerable<Guid> questionIds)
        {
            CheckInterviewExamContent(CompanyId, title, description, questionIds);
            Title = title;
            Description = description;
            SetUpdatedAt();
            SetQuestions(questionIds);
        }

        public void SetQuestions(IEnumerable<Guid> questionIds)
        {
            CheckQuestionIds(questionIds);
            Questions.Clear();

            int displayOrder = 1;
            foreach (Guid questionId in questionIds.Distinct())
            {
                InterviewExamQuestion examQuestion = new();
                examQuestion.CreateExamQuestion(Id, questionId, displayOrder);
                Questions.Add(examQuestion);
                displayOrder++;
            }
        }

        public void CheckInterviewExamContent(Guid companyId, string title, string description, IEnumerable<Guid> questionIds)
        {
            CheckGuid(companyId, nameof(companyId));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNull(description, nameof(description));
            CheckQuestionIds(questionIds);
        }

        private static void CheckQuestionIds(IEnumerable<Guid> questionIds)
        {
            ArgumentNullException.ThrowIfNull(questionIds, nameof(questionIds));

            if (questionIds.Any(questionId => questionId == Guid.Empty))
            {
                throw new ArgumentException("QuestionIds boş Guid içeremez.", nameof(questionIds));
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
