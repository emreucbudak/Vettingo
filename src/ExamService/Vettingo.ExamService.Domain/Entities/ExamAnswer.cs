using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class ExamAnswer : BaseEntity
    {
        public ExamAnswer()
        {
        }

        public Guid ExamAttemptId { get; private set; }
        public ExamAttempt? ExamAttempt { get; private set; }
        public Guid? MultipleChoiceQuestionId { get; private set; }
        public MultipleChoiceQuestion? MultipleChoiceQuestion { get; private set; }
        public Guid? MultipleChoiceOptionId { get; private set; }
        public MultipleChoiceOption? MultipleChoiceOption { get; private set; }
        public Guid? TrueFalseQuestionId { get; private set; }
        public TrueFalseQuestion? TrueFalseQuestion { get; private set; }
        public string AnswerText { get; private set; } = string.Empty;
        public bool? BooleanAnswer { get; private set; }
        public bool? IsCorrect { get; private set; }

        public void CreateMultipleChoiceAnswer(Guid examAttemptId, Guid questionId, Guid? optionId, string answerText, bool? isCorrect)
        {
            CheckMultipleChoiceAnswerContent(examAttemptId, questionId, optionId, answerText);
            SetId();
            SetCreatedAt();
            ExamAttemptId = examAttemptId;
            MultipleChoiceQuestionId = questionId;
            MultipleChoiceOptionId = optionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }

        public void CreateTrueFalseAnswer(Guid examAttemptId, Guid questionId, bool booleanAnswer, bool? isCorrect)
        {
            CheckAnswerIds(examAttemptId, questionId);
            SetId();
            SetCreatedAt();
            ExamAttemptId = examAttemptId;
            TrueFalseQuestionId = questionId;
            BooleanAnswer = booleanAnswer;
            AnswerText = booleanAnswer.ToString();
            IsCorrect = isCorrect;
        }

        public void CheckMultipleChoiceAnswerContent(Guid examAttemptId, Guid questionId, Guid? optionId, string answerText)
        {
            CheckAnswerIds(examAttemptId, questionId);
            CheckNullableGuid(optionId, nameof(optionId));
            ArgumentNullException.ThrowIfNull(answerText, nameof(answerText));

            if (optionId is null && string.IsNullOrWhiteSpace(answerText))
            {
                throw new ArgumentException("Çoktan seçmeli cevap optionId veya answerText içermelidir.", nameof(answerText));
            }
        }

        public void CheckAnswerIds(Guid examAttemptId, Guid questionId)
        {
            CheckGuid(examAttemptId, nameof(examAttemptId));
            CheckGuid(questionId, nameof(questionId));
        }

        private static void CheckGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }

        private static void CheckNullableGuid(Guid? value, string parameterName)
        {
            if (value.HasValue && value.Value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }
    }
}
