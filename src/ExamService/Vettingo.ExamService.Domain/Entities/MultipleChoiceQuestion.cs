using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class MultipleChoiceQuestion : BaseEntity
    {
        public MultipleChoiceQuestion()
        {
        }

        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public int Weight { get; private set; }
        public int DisplayOrder { get; private set; }
        public List<MultipleChoiceOption> Options { get; private set; } = new();

        public void CreateQuestion(Guid examId, string questionText, int weight, int displayOrder)
        {
            CheckMultipleChoiceQuestionContent(examId, questionText, weight, displayOrder);
            SetId();
            SetCreatedAt();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder);
        }

        public void UpdateQuestion(string questionText, int weight, int displayOrder)
        {
            CheckMultipleChoiceQuestionContent(questionText, weight, displayOrder);
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
        }

        public void AddOption(MultipleChoiceOption option)
        {
            ArgumentNullException.ThrowIfNull(option, nameof(option));
            Options.Add(option);
        }

        public void ClearOptions()
        {
            Options.Clear();
        }

        public void CheckMultipleChoiceQuestionContent(Guid examId, string questionText, int weight, int displayOrder)
        {
            CheckGuid(examId, nameof(examId));
            CheckMultipleChoiceQuestionContent(questionText, weight, displayOrder);
        }

        public void CheckMultipleChoiceQuestionContent(string questionText, int weight, int displayOrder)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionText, nameof(questionText));

            if (weight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), weight, "Ağırlık sıfırdan büyük olmalıdır.");
            }

            if (displayOrder <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(displayOrder), displayOrder, "Gösterim sırası sıfırdan büyük olmalıdır.");
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
