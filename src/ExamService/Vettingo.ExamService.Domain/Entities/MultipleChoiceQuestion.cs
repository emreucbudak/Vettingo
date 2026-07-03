namespace Vettingo.ExamService.Domain.Entities
{
    public class MultipleChoiceQuestion
    {
        public MultipleChoiceQuestion()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public decimal Weight { get; private set; }
        public int DisplayOrder { get; private set; }
        public string Explanation { get; private set; } = string.Empty;
        public List<MultipleChoiceOption> Options { get; private set; } = new();
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateQuestion(Guid examId, string questionText,int point, decimal weight, int displayOrder, string explanation)
        {
            SetId();
            ExamId = examId;
            UpdateQuestion(questionText,weight, displayOrder, explanation);
        }

        public void UpdateQuestion(string questionText,decimal weight, int displayOrder, string explanation)
        {
            QuestionText = questionText;
            Weight = weight;
            DisplayOrder = displayOrder;
            Explanation = explanation;
        }

        public void AddOption(MultipleChoiceOption option)
        {
            Options.Add(option);
        }

        public void ClearOptions()
        {
            Options.Clear();
        }
    }
}
