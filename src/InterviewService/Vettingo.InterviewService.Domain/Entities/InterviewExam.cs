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
            Title = title;
            Description = description;
            SetUpdatedAt();
            SetQuestions(questionIds);
        }

        public void SetQuestions(IEnumerable<Guid> questionIds)
        {
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
    }
}
