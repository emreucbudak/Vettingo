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

        public void CreateQuestion(Guid examId, string questionText, string topic, int point, decimal weight, int displayOrder, string explanation, bool correctAnswer)
        {
            SetId();
            ExamId = examId;
            UpdateQuestion(questionText, weight, displayOrder, explanation, correctAnswer);
        }

        public void UpdateQuestion(string questionText,decimal weight, int displayOrder, string explanation, bool correctAnswer)
        {
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            Explanation = explanation;
            CorrectAnswer = correctAnswer;
        }
    }
}
