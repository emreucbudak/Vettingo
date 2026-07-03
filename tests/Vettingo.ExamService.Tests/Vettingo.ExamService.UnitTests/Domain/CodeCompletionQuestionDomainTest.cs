using FluentAssertions;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class CodeCompletionQuestionDomainTest
    {
        [Fact]
        public async Task Create_CodeCompletionQuestion_With_Valid_Parameters()
        {
            // Arrange
            Vettingo.ExamService.Domain.Entities.CodeCompletionQuestion codeCompletionQuestion = new Vettingo.ExamService.Domain.Entities.CodeCompletionQuestion();
            // Act
            Action action = () =>
            {
                codeCompletionQuestion.CreateQuestion(Guid.NewGuid(), "Test Question",1.8m,1,"Test Explanation","Test Code Snippet","Test Expected Answer");
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
