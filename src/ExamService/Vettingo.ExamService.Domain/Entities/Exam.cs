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

        public Exam()
        {
        }

        public Guid? CompanyId { get; private set; }
        public ExamOwnerType OwnerType { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public int DurationMinutes { get; private set; }
        public int PassingScore { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<MultipleChoiceQuestion> MultipleChoiceQuestions => _multipleChoiceQuestions.AsReadOnly();
        public IReadOnlyCollection<TrueFalseQuestion> TrueFalseQuestions => _trueFalseQuestions.AsReadOnly();
        
        public void UpdateExamActivate()
        {
            IsActive = false;
        }
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

        public void UpdateMultipleChoiceQuestions(IReadOnlyCollection<MultipleChoiceQuestion> questions)
        {
            ArgumentNullException.ThrowIfNull(questions);
            _multipleChoiceQuestions.Clear();
            _multipleChoiceQuestions.AddRange(questions);
        }

        public void UpdateTrueFalseQuestions(IReadOnlyCollection<TrueFalseQuestion> questions)
        {
            ArgumentNullException.ThrowIfNull(questions);
            _trueFalseQuestions.Clear();
            _trueFalseQuestions.AddRange(questions);
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
            if (passingScore is < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(passingScore), passingScore, "Geçme puanı negatif olamaz.");
            }

            PassingScore = passingScore;
        }

        public void CreateExam(string title, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId)
        {
            CheckExamContent(title, durationMinutes, passingScore, ownerType, companyId);
            SetId();
            setTitle(title);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setOwnerType(ownerType);
            setCompanyId(companyId);
            SetCreatedAt();
            IsActive = true;
        }

        public void UpdateExam(string title, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId)
        {
            CheckExamContent(title, durationMinutes, passingScore, ownerType, companyId);
            setTitle(title);
            setDurationMinutes(durationMinutes);
            setPassingScore(passingScore);
            setOwnerType(ownerType);
            setCompanyId(companyId);
            UpdatedAt = DateTime.UtcNow;
        }

        public void CheckExamContent(string title, int durationMinutes, int passingScore, ExamOwnerType ownerType, Guid? companyId)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
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

            if (passingScore is < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(passingScore), passingScore, "Geçme puanı negatif olamaz.");
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
