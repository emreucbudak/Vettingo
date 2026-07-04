using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class MultipleChoiceQuestionDomainTest
    {
        [Fact]
        public void Create_MultipleChoiceQuestion_With_Valid_Parameters()
        {
            // Arrange
            var examId = Guid.NewGuid();
            var questionText = "Turkiyenin baskenti neresidir?";
            var weight = 1.0m;
            var displayOrder = 1;
            var explanation = "Turkiyenin baskenti merkez sehridir.";

            // Act
            Action action = () =>
            {
                var question = new MultipleChoiceQuestion();
                question.CreateQuestion(examId, questionText, weight, displayOrder, explanation);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_MultipleChoiceQuestion_With_Empty_QuestionText_Should_Throw()
        {
            // Act
            Action action = () =>
            {
                var question = new MultipleChoiceQuestion();
                question.CreateQuestion(Guid.NewGuid(), string.Empty, 1m, 1, "Explanation");
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_MultipleChoiceQuestion_With_Zero_Weight_Should_Throw()
        {
            // Act
            Action action = () =>
            {
                var question = new MultipleChoiceQuestion();
                question.CreateQuestion(Guid.NewGuid(), "Question", 0m, 1, "Explanation");
            };

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
