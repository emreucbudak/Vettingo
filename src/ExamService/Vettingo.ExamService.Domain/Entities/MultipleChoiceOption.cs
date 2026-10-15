using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class MultipleChoiceOption : BaseEntity
    {
        public MultipleChoiceOption()
        {
        }

        public Guid MultipleChoiceQuestionId { get; private set; }
        public MultipleChoiceQuestion? MultipleChoiceQuestion { get; private set; }
        public string OptionText { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }
        public int DisplayOrder { get; private set; }
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void CreateOption(Guid multipleChoiceQuestionId, string optionText, bool isCorrect, int displayOrder)
        {
            CheckMultipleChoiceOptionContent(multipleChoiceQuestionId, optionText, displayOrder);
            SetId();
            SetCreatedAt();
            MultipleChoiceQuestionId = multipleChoiceQuestionId;
            OptionText = optionText;
            IsCorrect = isCorrect;
            DisplayOrder = displayOrder;
        }

        public void CheckMultipleChoiceOptionContent(Guid multipleChoiceQuestionId, string optionText, int displayOrder)
        {
            if (multipleChoiceQuestionId == Guid.Empty)
            {
                throw new ArgumentException("MultipleChoiceQuestionId boş olamaz.", nameof(multipleChoiceQuestionId));
            }

            ArgumentNullException.ThrowIfNullOrWhiteSpace(optionText, nameof(optionText));

            if (displayOrder <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(displayOrder), displayOrder, "Gösterim sırası sıfırdan büyük olmalıdır.");
            }
        }
    }
}
