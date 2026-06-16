namespace Vettingo.ExamService.Domain.Entities
{
    public class Exam
    {
        public Exam()
        {
        }

        public Guid Id { get; private set; }
        public Guid? JobId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int DurationMinutes { get; private set; }
        public int PassingScore { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public List<Question> Questions { get; private set; } = new();
        public List<ExamAttempt> Attempts { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void setJobId(Guid? jobId)
        {
            JobId = jobId;
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

        public void CreateExam(string title, string subject, string description, int durationMinutes, int passingScore, Guid? jobId)
        {
            SetId();
            setTitle(title);
            setSubject(subject);
            setDescription(description);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setJobId(jobId);
            setIsActive(true);
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateExam(string title, string subject, string description, int durationMinutes, int passingScore, Guid? jobId, bool isActive)
        {
            setTitle(title);
            setSubject(subject);
            setDescription(description);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setJobId(jobId);
            setIsActive(isActive);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
