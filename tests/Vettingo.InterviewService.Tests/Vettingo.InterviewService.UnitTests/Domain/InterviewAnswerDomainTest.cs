using FluentAssertions;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewAnswerDomainTest
    {
        [Fact]
        public void Create_InterviewAnswer_With_Valid_Parameters()
        {
            // Arrange
            InterviewAnswer interviewAnswer = new();

            // Act
            Action action = () =>
            {
                interviewAnswer.CreateAnswer(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow));
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_InterviewAnswer_With_Empty_UserId_Should_Throw()
        {
            // Arrange
            InterviewAnswer interviewAnswer = new();

            // Act
            Action action = () =>
            {
                interviewAnswer.CreateAnswer(Guid.Empty, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow));
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_InterviewAnswer_With_Default_AnswerDate_Should_Throw()
        {
            // Arrange
            InterviewAnswer interviewAnswer = new();

            // Act
            Action action = () =>
            {
                interviewAnswer.CreateAnswer(Guid.NewGuid(), Guid.NewGuid(), default);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
