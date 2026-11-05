using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    [Index(nameof(Title))]
    public class Exam : BaseEntity
    {
        private readonly List<MultipleChoiceQuestion> _multipleChoiceQuestions = new();
        private readonly List<TrueFalseQuestion> _trueFalseQuestions = new();
        private readonly List<ExamAttempt> _attempts = new();

        public Exam()
        {
        }

        public Guid? CompanyId { get; private set; }
        public ExamOwnerType OwnerType { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int DurationMinutes { get; private set; }
        public int PassingScore { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<MultipleChoiceQuestion> MultipleChoiceQuestions => _multipleChoiceQuestions.AsReadOnly();
        public IReadOnlyCollection<TrueFalseQuestion> TrueFalseQuestions => _trueFalseQuestions.AsReadOnly();
        public IReadOnlyCollection<ExamAttempt> Attempts => _attempts.AsReadOnly();

        public IReadOnlyCollection<MultipleChoiceQuestion> GetMultipleChoiceQuestions()
        {
            return _multipleChoiceQuestions.AsReadOnly();
        }

        public IReadOnlyCollection<TrueFalseQuestion> GetTrueFalseQuestions()
        {
            return _trueFalseQuestions.AsReadOnly();
        }

        public void AddMultipleChoiceQuestion(MultipleChoiceQuestion question)
        {
            ArgumentNullException.ThrowIfNull(question, nameof(question));
            _multipleChoiceQuestions.Add(question);
        }

        public void AddTrueFalseQuestion(TrueFalseQuestion question)
        {
            ArgumentNullException.ThrowIfNull(question, nameof(question));
            _trueFalseQuestions.Add(question);
        }

        public bool RemoveMultipleChoiceQuestion(Guid questionId)
        {
            var question = _multipleChoiceQuestions.FirstOrDefault(question => question.Id == questionId);
            return question is not null && _multipleChoiceQuestions.Remove(question);
        }

        public bool RemoveTrueFalseQuestion(Guid questionId)
        {
            var question = _trueFalseQuestions.FirstOrDefault(question => question.Id == questionId);
            return question is not null && _trueFalseQuestions.Remove(question);
        }

        public void AddAttempt(ExamAttempt attempt)
        {
            ArgumentNullException.ThrowIfNull(attempt, nameof(attempt));
            _attempts.Add(attempt);
        }

        public void setCompanyId(Guid? companyId)
        {
            CheckNullableGuid(companyId, nameof(companyId));
            CompanyId = companyId;
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

        public void CreateExam(string title, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId)
        {
            CheckExamContent(title, description, durationMinutes, passingScore, ownerType, companyId);
            SetId();
            setTitle(title);
            setDescription(description);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setOwnerType(ownerType);
            setCompanyId(companyId);
            SetCreatedAt();
        }

        public void UpdateExam(string title, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId)
        {
            CheckExamContent(title, description, durationMinutes, passingScore, ownerType, companyId);
            setTitle(title);
            setDescription(description);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setOwnerType(ownerType);
            setCompanyId(companyId);
            UpdatedAt = DateTime.UtcNow;
        }

        public void CheckExamContent(string title, string description, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description, nameof(description));
            CheckOwnerType(ownerType);
            CheckNullableGuid(companyId, nameof(companyId));

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
