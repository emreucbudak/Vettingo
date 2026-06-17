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
        public Guid? MultipleChoiceQuestionId { get; private set; }
        public MultipleChoiceQuestion? MultipleChoiceQuestion { get; private set; }
        public Guid? MultipleChoiceOptionId { get; private set; }
        public MultipleChoiceOption? MultipleChoiceOption { get; private set; }
        public Guid? TrueFalseQuestionId { get; private set; }
        public TrueFalseQuestion? TrueFalseQuestion { get; private set; }
        public Guid? ClassicQuestionId { get; private set; }
        public ClassicQuestion? ClassicQuestion { get; private set; }
        public Guid? CodeCompletionQuestionId { get; private set; }
        public CodeCompletionQuestion? CodeCompletionQuestion { get; private set; }
        public string AnswerText { get; private set; } = string.Empty;
        public bool? BooleanAnswer { get; private set; }
        public bool? IsCorrect { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateMultipleChoiceAnswer(Guid examAttemptId, Guid questionId, Guid? optionId, string answerText, bool? isCorrect)
        {
            SetId();
            ExamAttemptId = examAttemptId;
            MultipleChoiceQuestionId = questionId;
            MultipleChoiceOptionId = optionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }

        public void CreateTrueFalseAnswer(Guid examAttemptId, Guid questionId, bool booleanAnswer, bool? isCorrect)
        {
            SetId();
            ExamAttemptId = examAttemptId;
            TrueFalseQuestionId = questionId;
            BooleanAnswer = booleanAnswer;
            AnswerText = booleanAnswer.ToString();
            IsCorrect = isCorrect;
        }

        public void CreateClassicAnswer(Guid examAttemptId, Guid questionId, string answerText, bool? isCorrect)
        {
            SetId();
            ExamAttemptId = examAttemptId;
            ClassicQuestionId = questionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }

        public void CreateCodeCompletionAnswer(Guid examAttemptId, Guid questionId, string answerText, bool? isCorrect)
        {
            SetId();
            ExamAttemptId = examAttemptId;
            CodeCompletionQuestionId = questionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }
    }
}
