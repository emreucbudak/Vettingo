using Vettingo.InterviewService.Domain.Common;
using Vettingo.InterviewService.Domain.Enums;

namespace Vettingo.InterviewService.Domain.Entities
{
    public class InterviewExam : BaseEntity
    {
        public InterviewExam()
        {
        }

        public Guid CompanyId { get; private set; }
        public Guid CandidateId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public InterviewType Type { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ICollection<InterviewExamQuestion> Questions { get; private set; } = new List<InterviewExamQuestion>();
        public ICollection<InterviewAnswer> Answers { get; private set; } = new List<InterviewAnswer>();

        public void CreateExam(
            Guid companyId,
            Guid candidateId,
            string title,
            string description,
            InterviewType type,
            DateTime startDate,
            DateTime? endDate,
            IEnumerable<Guid> questionIds)
        {
            CheckInterviewExamContent(companyId, candidateId, title, description, type, startDate, endDate, questionIds);
            SetId();
            CompanyId = companyId;
            CandidateId = candidateId;
            Title = title;
            Description = description;
            Type = type;
            StartDate = NormalizeUtc(startDate);
            EndDate = endDate.HasValue ? NormalizeUtc(endDate.Value) : null;
            SetCreatedAt();
            UpdatedAt = null;
            SetQuestions(questionIds);
        }

        public void UpdateExam(
            Guid candidateId,
            string title,
            string description,
            InterviewType type,
            DateTime startDate,
            DateTime? endDate,
            IEnumerable<Guid> questionIds)
        {
            CheckInterviewExamContent(CompanyId, candidateId, title, description, type, startDate, endDate, questionIds);
            CandidateId = candidateId;
            Title = title;
            Description = description;
            Type = type;
            StartDate = NormalizeUtc(startDate);
            EndDate = endDate.HasValue ? NormalizeUtc(endDate.Value) : null;
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

        public void CheckInterviewExamContent(
            Guid companyId,
            Guid candidateId,
            string title,
            string description,
            InterviewType type,
            DateTime startDate,
            DateTime? endDate,
            IEnumerable<Guid> questionIds)
        {
            CheckGuid(companyId, nameof(companyId));
            CheckGuid(candidateId, nameof(candidateId));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNull(description, nameof(description));
            CheckQuestionIds(questionIds);

            if (!Enum.IsDefined(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, "Mülakat tipi geçersiz.");
            }

            if (startDate == default)
            {
                throw new ArgumentException("Mülakat başlangıç tarihi geçersiz.", nameof(startDate));
            }

            if (type == InterviewType.AI && !endDate.HasValue)
            {
                throw new ArgumentException("AI mülakatlarında bitiş tarihi zorunludur.", nameof(endDate));
            }

            if (endDate.HasValue && NormalizeUtc(endDate.Value) <= NormalizeUtc(startDate))
            {
                throw new ArgumentException("Mülakat bitiş tarihi başlangıç tarihinden sonra olmalıdır.", nameof(endDate));
            }
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

        private static DateTime NormalizeUtc(DateTime value) =>
            value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
    }
}
