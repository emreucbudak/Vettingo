using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    public class TrueFalseOption : BaseEntity
    {
        public Guid TrueFalseQuestionId { get; private set; }
        public TrueFalseQuestion TrueFalseQuestion { get; private set; } = null!;
        public TrueFalseOptionType Type { get; private set; }
        public bool IsCorrect { get; private set; }

        public void CreateOption(Guid trueFalseQuestionId, TrueFalseOptionType type, bool isCorrect)
        {
            if (trueFalseQuestionId == Guid.Empty)
            {
                throw new ArgumentException("TrueFalseQuestionId boş olamaz.", nameof(trueFalseQuestionId));
            }

            if (!Enum.IsDefined(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, "Geçersiz doğru/yanlış seçenek türü.");
            }

            SetId();
            SetCreatedAt();
            TrueFalseQuestionId = trueFalseQuestionId;
            Type = type;
            IsCorrect = isCorrect;
        }
    }
}
