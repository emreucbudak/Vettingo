using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class MultipleChoiceOption : BaseEntity
    {
        public MultipleChoiceOption()
        {
        }

        public Guid MultipleChoiceQuestionId { get; private set; }
        public MultipleChoiceQuestion MultipleChoiceQuestion { get; private set; } = null!;
        public string OptionText { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }

        public void CreateOption(Guid multipleChoiceQuestionId, string optionText, bool isCorrect)
        {
            CheckMultipleChoiceOptionContent(multipleChoiceQuestionId, optionText);
            SetId();
            SetCreatedAt();
            MultipleChoiceQuestionId = multipleChoiceQuestionId;
            OptionText = optionText;
            IsCorrect = isCorrect;
        }

        public void CheckMultipleChoiceOptionContent(Guid multipleChoiceQuestionId, string optionText)
        {
            if (multipleChoiceQuestionId == Guid.Empty)
            {
                throw new ArgumentException("MultipleChoiceQuestionId boş olamaz.", nameof(multipleChoiceQuestionId));
            }

            ArgumentNullException.ThrowIfNullOrWhiteSpace(optionText, nameof(optionText));
        }
    }
}
