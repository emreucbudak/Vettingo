using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ClassicQuestionDomainTest
    {
        [Fact]
        public void Create_ClassicQuestion_With_Valid_Parameters()
        {
            // Arrange
            ClassicQuestion classicQuestion = new();

            // Act
            Action action = () =>
            {
                classicQuestion.CreateQuestion(Guid.NewGuid(), "Sample Question", 1.6m, 2, "Sample explanation", "Sample Answer");
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_ClassicQuestion_With_Empty_ExamId_Should_Throw()
        {
            // Arrange
            ClassicQuestion classicQuestion = new();

            // Act
            Action action = () =>
            {
                classicQuestion.CreateQuestion(Guid.Empty, "Sample Question", 1.6m, 2, "Sample explanation", "Sample Answer");
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ClassicQuestion_With_Zero_Weight_Should_Throw()
        {
            // Arrange
            ClassicQuestion classicQuestion = new();

            // Act
            Action action = () =>
            {
                classicQuestion.CreateQuestion(Guid.NewGuid(), "Sample Question", 0m, 2, "Sample explanation", "Sample Answer");
            };

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
