using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class MultipleChoiceQuestionDomainTest
    {
        [Fact]
        public async Task Create_MultipleChoiceQuestion_With_Valid_Parameters()
        {
            // Arrange
            var examId = Guid.NewGuid();
            var questionText = "Türkiyenin başkenti neresidir?";
            var point = 5;
            var weight = 1.0m;
            var displayOrder = 1;
            var explanation = "Türkiyenin başkenti merkez şehridir.";
            // Act
            Action act = () =>
            {
                var question = new MultipleChoiceQuestion();
                question.CreateQuestion(examId, questionText,point, weight, displayOrder, explanation);
            };
            // Assert
            act.Should().NotThrow();
        }
    }
}
