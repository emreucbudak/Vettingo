using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class CodeCompletionQuestionDomainTest
    {
        [Fact]
        public void Create_CodeCompletionQuestion_With_Valid_Parameters()
        {
            // Arrange
            CodeCompletionQuestion codeCompletionQuestion = new();

            // Act
            Action action = () =>
            {
                codeCompletionQuestion.CreateQuestion(Guid.NewGuid(), "Test Question", 1.8m, 1, "Test Explanation", "Test Code Snippet", "Test Expected Answer");
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_CodeCompletionQuestion_With_Empty_CodeSnippet_Should_Throw()
        {
            // Arrange
            CodeCompletionQuestion codeCompletionQuestion = new();

            // Act
            Action action = () =>
            {
                codeCompletionQuestion.CreateQuestion(Guid.NewGuid(), "Test Question", 1.8m, 1, "Test Explanation", string.Empty, "Test Expected Answer");
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_CodeCompletionQuestion_With_Invalid_DisplayOrder_Should_Throw()
        {
            // Arrange
            CodeCompletionQuestion codeCompletionQuestion = new();

            // Act
            Action action = () =>
            {
                codeCompletionQuestion.CreateQuestion(Guid.NewGuid(), "Test Question", 1.8m, 0, "Test Explanation", "Test Code Snippet", "Test Expected Answer");
            };

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
