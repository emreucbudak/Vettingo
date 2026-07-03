using FluentAssertions;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewQuestionDomainTest
    {
        [Fact]
        public async Task Create_InterviewQuestion_With_Valid_Parameters()
        {
            // Arrange
            InterviewService.Domain.Entities.InterviewQuestion interviewQuestion = new InterviewService.Domain.Entities.InterviewQuestion();
            // Act
            Action action = () =>
            {
                interviewQuestion.CreateQuestion(Guid.NewGuid(), "En güçlü yanınız nedir?");
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
