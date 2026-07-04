namespace Vettingo.ExamService.Domain.Entities
{
    public class CodeCompletionQuestion
    {
        public CodeCompletionQuestion()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public decimal Weight { get; private set; }
        public int DisplayOrder { get; private set; }
        public string Explanation { get; private set; } = string.Empty;
        public string CodeSnippet { get; private set; } = string.Empty;
        public string ExpectedAnswer { get; private set; } = string.Empty;
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateQuestion(Guid examId, string questionText, decimal weight, int displayOrder, string explanation, string codeSnippet, string expectedAnswer)
        {
            CheckCodeCompletionQuestionContent(examId, questionText, weight, displayOrder, explanation, codeSnippet, expectedAnswer);
            SetId();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder, explanation, codeSnippet, expectedAnswer);
        }

        public void UpdateQuestion(string questionText, decimal weight, int displayOrder, string explanation, string codeSnippet, string expectedAnswer)
        {
            CheckCodeCompletionQuestionContent(questionText, weight, displayOrder, explanation, codeSnippet, expectedAnswer);
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            Explanation = explanation;
            CodeSnippet = codeSnippet;
            ExpectedAnswer = expectedAnswer;
        }

        public void CheckCodeCompletionQuestionContent(Guid examId, string questionText, decimal weight, int displayOrder, string explanation, string codeSnippet, string expectedAnswer)
        {
            CheckGuid(examId, nameof(examId));
            CheckCodeCompletionQuestionContent(questionText, weight, displayOrder, explanation, codeSnippet, expectedAnswer);
        }

        public void CheckCodeCompletionQuestionContent(string questionText, decimal weight, int displayOrder, string explanation, string codeSnippet, string expectedAnswer)
        {
            CheckQuestionContent(questionText, weight, displayOrder, explanation);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(codeSnippet, nameof(codeSnippet));
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
