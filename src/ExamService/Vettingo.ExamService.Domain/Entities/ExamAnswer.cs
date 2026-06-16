namespace Vettingo.ExamService.Domain.Entities
{
    public class ExamAnswer
    {
        public ExamAnswer()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamAttemptId { get; private set; }
        public ExamAttempt? ExamAttempt { get; private set; }
        public Guid QuestionId { get; private set; }
        public Question? Question { get; private set; }
        public Guid? QuestionOptionId { get; private set; }
        public QuestionOption? QuestionOption { get; private set; }
        public string AnswerText { get; private set; } = string.Empty;
        public bool? IsCorrect { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateAnswer(Guid examAttemptId, Guid questionId, Guid? questionOptionId, string answerText, bool? isCorrect)
        {
            SetId();
            ExamAttemptId = examAttemptId;
            QuestionId = questionId;
            QuestionOptionId = questionOptionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }
    }
}
