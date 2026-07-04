using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class MultipleChoiceOptionDomainTest
    {
        [Fact]
        public void Create_MultipleChoiceOption_With_Valid_Parameters()
        {
            // Arrange
            var multipleChoiceQuestionId = Guid.NewGuid();
            var optionText = "Ankara";
            var isCorrect = true;
            var displayOrder = 1;

            // Act
            Action action = () =>
            {
                var option = new MultipleChoiceOption();
                option.CreateOption(multipleChoiceQuestionId, optionText, isCorrect, displayOrder);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_MultipleChoiceOption_With_Empty_QuestionId_Should_Throw()
        {
            // Act
            Action action = () =>
            {
                var option = new MultipleChoiceOption();
                option.CreateOption(Guid.Empty, "Ankara", true, 1);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_MultipleChoiceOption_With_Empty_Text_Should_Throw()
        {
            // Act
            Action action = () =>
            {
                var option = new MultipleChoiceOption();
                option.CreateOption(Guid.NewGuid(), string.Empty, true, 1);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
