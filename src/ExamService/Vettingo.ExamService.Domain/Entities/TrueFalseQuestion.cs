namespace Vettingo.ExamService.Domain.Entities
{
    public class TrueFalseQuestion
    {
        public TrueFalseQuestion()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public decimal Weight { get; private set; }
        public int DisplayOrder { get; private set; }
        public string Explanation { get; private set; } = string.Empty;
        public bool CorrectAnswer { get; private set; }
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateQuestion(Guid examId, string questionText, decimal weight, int displayOrder, string explanation, bool correctAnswer)
        {
            CheckTrueFalseQuestionContent(examId, questionText, weight, displayOrder, explanation);
            SetId();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder, explanation, correctAnswer);
        }

        public void UpdateQuestion(string questionText, decimal weight, int displayOrder, string explanation, bool correctAnswer)
        {
            CheckTrueFalseQuestionContent(questionText, weight, displayOrder, explanation);
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            Explanation = explanation;
            CorrectAnswer = correctAnswer;
        }

        public void CheckTrueFalseQuestionContent(Guid examId, string questionText, decimal weight, int displayOrder, string explanation)
        {
            CheckGuid(examId, nameof(examId));
            CheckTrueFalseQuestionContent(questionText, weight, displayOrder, explanation);
        }

        public void CheckTrueFalseQuestionContent(string questionText, decimal weight, int displayOrder, string explanation)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(questionText, nameof(questionText));
            ArgumentNullException.ThrowIfNull(explanation, nameof(explanation));

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
