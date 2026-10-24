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
        public string TrueAnswer { get; private set; } = string.Empty;
        public string FalseAnswer { get; private set; } = string.Empty;
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void CreateQuestion(Guid examId, string questionText, int weight, int displayOrder, string trueAnswer, string falseAnswer)
        {
            CheckTrueFalseQuestionContent(examId, questionText, weight, displayOrder, trueAnswer, falseAnswer);
            SetId();
            SetCreatedAt();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder, trueAnswer, falseAnswer);
        }

        public void UpdateQuestion(string questionText, int weight, int displayOrder, string trueAnswer, string falseAnswer)
        {
            CheckTrueFalseQuestionContent(questionText, weight, displayOrder, trueAnswer, falseAnswer);
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            TrueAnswer = trueAnswer;
            FalseAnswer = falseAnswer;
        }

        public void CheckTrueFalseQuestionContent(Guid examId, string questionText, int weight, int displayOrder, string trueAnswer, string falseAnswer)
        {
            CheckGuid(examId, nameof(examId));
            CheckTrueFalseQuestionContent(questionText, weight, displayOrder, trueAnswer, falseAnswer);
        }

        public void CheckTrueFalseQuestionContent(string questionText, int weight, int displayOrder, string trueAnswer, string falseAnswer)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionText, nameof(questionText));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(trueAnswer, nameof(trueAnswer));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(falseAnswer, nameof(falseAnswer));

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
