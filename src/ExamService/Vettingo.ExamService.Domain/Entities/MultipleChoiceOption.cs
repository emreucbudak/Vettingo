using System.ComponentModel.DataAnnotations.Schema;
using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Enums;

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

        [NotMapped]
        public MultipleChoiceOptionOrder? Order { get; private set; }

        public void CreateOption(Guid multipleChoiceQuestionId, string optionText, bool isCorrect, MultipleChoiceOptionOrder? order = null)
        {
            CheckMultipleChoiceOptionContent(multipleChoiceQuestionId, optionText);
            if (order.HasValue && !Enum.IsDefined(order.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(order), order, "Geçersiz seçenek sırası.");
            }

            SetId();
            SetCreatedAt();
            MultipleChoiceQuestionId = multipleChoiceQuestionId;
            OptionText = optionText;
            IsCorrect = isCorrect;
            Order = order;
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
