namespace Vettingo.ExamService.Domain.Entities
{
    public class ClassicQuestion
    {
        public ClassicQuestion()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public decimal Weight { get; private set; }
        public int DisplayOrder { get; private set; }
        public string Explanation { get; private set; } = string.Empty;
        public string ExpectedAnswer { get; private set; } = string.Empty;
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateQuestion(Guid examId, string questionText, decimal weight, int displayOrder, string explanation, string expectedAnswer)
        {
            CheckClassicQuestionContent(examId, questionText, weight, displayOrder, explanation, expectedAnswer);
            SetId();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder, explanation, expectedAnswer);
        }

        public void UpdateQuestion(string questionText, decimal weight, int displayOrder, string explanation, string expectedAnswer)
        {
            CheckClassicQuestionContent(questionText, weight, displayOrder, explanation, expectedAnswer);
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            Explanation = explanation;
            ExpectedAnswer = expectedAnswer;
        }

        public void CheckClassicQuestionContent(Guid examId, string questionText, decimal weight, int displayOrder, string explanation, string expectedAnswer)
        {
            CheckGuid(examId, nameof(examId));
            CheckClassicQuestionContent(questionText, weight, displayOrder, explanation, expectedAnswer);
        }

        public void CheckClassicQuestionContent(string questionText, decimal weight, int displayOrder, string explanation, string expectedAnswer)
        {
            CheckQuestionContent(questionText, weight, displayOrder, explanation);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(expectedAnswer, nameof(expectedAnswer));
        }

        private static void CheckQuestionContent(string questionText, decimal weight, int displayOrder, string explanation)
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
