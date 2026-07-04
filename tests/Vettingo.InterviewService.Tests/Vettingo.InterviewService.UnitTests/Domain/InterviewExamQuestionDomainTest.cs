using FluentAssertions;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewExamQuestionDomainTest
    {
        [Fact]
        public void Create_InterviewExamQuestion_With_Valid_Parameters()
        {
            // Arrange
            InterviewExamQuestion interviewExamQuestion = new();

            // Act
            Action action = () =>
            {
                interviewExamQuestion.CreateExamQuestion(Guid.NewGuid(), Guid.NewGuid(), 3);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_InterviewExamQuestion_With_Empty_InterviewExamId_Should_Throw()
        {
            // Arrange
            InterviewExamQuestion interviewExamQuestion = new();

            // Act
            Action action = () =>
            {
                interviewExamQuestion.CreateExamQuestion(Guid.Empty, Guid.NewGuid(), 3);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_InterviewExamQuestion_With_Invalid_DisplayOrder_Should_Throw()
        {
            // Arrange
            InterviewExamQuestion interviewExamQuestion = new();

            // Act
            Action action = () =>
            {
                interviewExamQuestion.CreateExamQuestion(Guid.NewGuid(), Guid.NewGuid(), 0);
            };

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
