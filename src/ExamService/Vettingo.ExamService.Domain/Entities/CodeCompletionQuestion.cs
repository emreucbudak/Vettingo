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
        public string Topic { get; private set; } = string.Empty;
        public int Point { get; private set; }
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

        public void CreateQuestion(Guid examId, string questionText, string topic, int point, decimal weight, int displayOrder, string explanation, string codeSnippet, string expectedAnswer)
        {
            SetId();
            ExamId = examId;
            UpdateQuestion(questionText, topic, point, weight, displayOrder, explanation, codeSnippet, expectedAnswer);
        }

        public void UpdateQuestion(string questionText, string topic, int point, decimal weight, int displayOrder, string explanation, string codeSnippet, string expectedAnswer)
        {
            QuestionText = questionText;
            Topic = topic;
            Point = point;
            Weight = weight;
            DisplayOrder = displayOrder;
            Explanation = explanation;
            CodeSnippet = codeSnippet;
            ExpectedAnswer = expectedAnswer;
        }
    }
}
