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
            CheckNullableGuid(companyId, nameof(companyId));
            CompanyId = companyId;
        }

        public void setJobId(Guid? jobId)
        {
            CheckNullableGuid(jobId, nameof(jobId));
            JobId = jobId;
        }

        public void setOwnerType(ExamOwnerType ownerType)
        {
            CheckOwnerType(ownerType);
            OwnerType = ownerType;
        }

        public void setTitle(string title)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            Title = title;
        }

        public void setSubject(string subject)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
            Subject = subject;
        }

        public void setDescription(string description)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description, nameof(description));
            Description = description;
        }

        public void setDurationMinutes(int durationMinutes)
        {
            if (durationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationMinutes), durationMinutes, "Süre sıfırdan büyük olmalıdır.");
            }

            DurationMinutes = durationMinutes;
        }

        public void setPassingScore(int passingScore)
        {
            if (passingScore is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(passingScore), passingScore, "Geçme puanı 0 ile 100 arasında olmalıdır.");
            }

            PassingScore = passingScore;
        }

        public void setIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void CreateExam(string title, string subject, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId, Guid? jobId)
        {
            CheckExamContent(title, subject, description, durationMinutes, passingScore, ownerType, companyId, jobId);
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
            CheckExamContent(title, subject, description, durationMinutes, passingScore, ownerType, companyId, jobId);
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

        public void CheckExamContent(string title, string subject, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId, Guid? jobId)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description, nameof(description));
            CheckOwnerType(ownerType);
            CheckNullableGuid(companyId, nameof(companyId));
            CheckNullableGuid(jobId, nameof(jobId));

            if (ownerType == ExamOwnerType.Company && companyId is null)
            {
                throw new ArgumentException("Şirket sınavları için CompanyId zorunludur.", nameof(companyId));
            }

            if (durationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationMinutes), durationMinutes, "Süre sıfırdan büyük olmalıdır.");
            }

            if (passingScore is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(passingScore), passingScore, "Geçme puanı 0 ile 100 arasında olmalıdır.");
            }
        }

        private static void CheckOwnerType(ExamOwnerType ownerType)
        {
            if (!Enum.IsDefined(typeof(ExamOwnerType), ownerType))
            {
                throw new ArgumentOutOfRangeException(nameof(ownerType), ownerType, "Sınav sahiplik tipi geçersiz.");
            }
        }

        private static void CheckNullableGuid(Guid? value, string parameterName)
        {
            if (value.HasValue && value.Value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }
    }
}
