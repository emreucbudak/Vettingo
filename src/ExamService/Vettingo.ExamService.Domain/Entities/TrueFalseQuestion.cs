using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class TrueFalseQuestion : BaseEntity
    {
        public TrueFalseQuestion()
        {
        }

        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public int Weight { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool CorrectAnswer { get; private set; }
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void CreateQuestion(Guid examId, string questionText, int weight, int displayOrder, bool correctAnswer)
        {
            CheckTrueFalseQuestionContent(examId, questionText, weight, displayOrder);
            SetId();
            SetCreatedAt();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder, correctAnswer);
        }

        public void UpdateQuestion(string questionText, int weight, int displayOrder, bool correctAnswer)
        {
            CheckTrueFalseQuestionContent(questionText, weight, displayOrder);
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            CorrectAnswer = correctAnswer;
        }

        public void CheckTrueFalseQuestionContent(Guid examId, string questionText, int weight, int displayOrder)
        {
            CheckGuid(examId, nameof(examId));
            CheckTrueFalseQuestionContent(questionText, weight, displayOrder);
        }

        public void CheckTrueFalseQuestionContent(string questionText, int weight, int displayOrder)
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
