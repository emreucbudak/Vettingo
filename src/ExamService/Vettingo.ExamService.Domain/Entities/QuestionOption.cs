namespace Vettingo.ExamService.Domain.Entities
{
    public class QuestionOption
    {
        public QuestionOption()
        {
        }

        public Guid Id { get; private set; }
        public Guid QuestionId { get; private set; }
        public Question? Question { get; private set; }
        public string OptionText { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }
        public int DisplayOrder { get; private set; }
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void setQuestionId(Guid questionId)
        {
            QuestionId = questionId;
        }

        public void setOptionText(string optionText)
        {
            OptionText = optionText;
        }

        public void setIsCorrect(bool isCorrect)
        {
            IsCorrect = isCorrect;
        }

        public void setDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
        }

        public void CreateOption(Guid questionId, string optionText, bool isCorrect, int displayOrder)
        {
            SetId();
            setQuestionId(questionId);
            setOptionText(optionText);
            setIsCorrect(isCorrect);
            setDisplayOrder(displayOrder);
        }
    }
}
