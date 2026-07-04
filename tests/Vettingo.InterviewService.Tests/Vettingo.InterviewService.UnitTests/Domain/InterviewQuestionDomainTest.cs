using FluentAssertions;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewQuestionDomainTest
    {
        [Fact]
        public void Create_InterviewQuestion_With_Valid_Parameters()
        {
            // Arrange
            InterviewQuestion interviewQuestion = new();

            // Act
            Action action = () =>
            {
                interviewQuestion.CreateQuestion(Guid.NewGuid(), "En guclu yaniniz nedir?");
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_InterviewQuestion_With_Empty_CompanyId_Should_Throw()
        {
            // Arrange
            InterviewQuestion interviewQuestion = new();

            // Act
            Action action = () =>
            {
                interviewQuestion.CreateQuestion(Guid.Empty, "En guclu yaniniz nedir?");
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_InterviewQuestion_With_Empty_QuestionText_Should_Throw()
        {
            // Arrange
            InterviewQuestion interviewQuestion = new();

            // Act
            Action action = () =>
            {
                interviewQuestion.CreateQuestion(Guid.NewGuid(), string.Empty);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
