using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    public class Exam
    {
        public Exam()
        {
        }

        public Guid Id { get; private set; }
        public Guid? CompanyId { get; private set; }
        public Guid? JobId { get; private set; }
        public ExamOwnerType OwnerType { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int DurationMinutes { get; private set; }
        public int PassingScore { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; private set; } = new();
        public List<TrueFalseQuestion> TrueFalseQuestions { get; private set; } = new();
        public List<ClassicQuestion> ClassicQuestions { get; private set; } = new();
        public List<CodeCompletionQuestion> CodeCompletionQuestions { get; private set; } = new();
        public List<ExamAttempt> Attempts { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void setCompanyId(Guid? companyId)
        {
            CompanyId = companyId;
        }

        public void setJobId(Guid? jobId)
        {
            JobId = jobId;
        }

        public void setOwnerType(ExamOwnerType ownerType)
        {
            OwnerType = ownerType;
        }

        public void setTitle(string title)
        {
            Title = title;
        }

        public void setSubject(string subject)
        {
            Subject = subject;
        }

        public void setDescription(string description)
        {
            Description = description;
        }

        public void setDurationMinutes(int durationMinutes)
        {
            DurationMinutes = durationMinutes;
        }

        public void setPassingScore(int passingScore)
        {
            PassingScore = passingScore;
        }

        public void setIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void CreateExam(string title, string subject, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId, Guid? jobId)
        {
            SetId();
            setTitle(title);
            setSubject(subject);
            setDescription(description);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setOwnerType(ownerType);
            setCompanyId(companyId);
            setJobId(jobId);
            setIsActive(true);
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateExam(string title, string subject, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId, Guid? jobId, bool isActive)
        {
            setTitle(title);
            setSubject(subject);
            setDescription(description);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setOwnerType(ownerType);
            setCompanyId(companyId);
            setJobId(jobId);
            setIsActive(isActive);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
