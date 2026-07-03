using FluentAssertions;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewAnswerDomainTest
    {
        [Fact]
        public async Task Create_InterviewAnswer_With_Valid_Parameters()
        {
            // Arrange
            InterviewService.Domain.Entities.InterviewAnswer interviewAnswer = new InterviewService.Domain.Entities.InterviewAnswer();
            // Act
            Action action = () =>
            {
                interviewAnswer.CreateAnswer(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow));
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
