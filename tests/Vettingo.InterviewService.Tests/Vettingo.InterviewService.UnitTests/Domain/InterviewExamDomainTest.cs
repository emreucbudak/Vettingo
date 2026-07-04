using FluentAssertions;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewExamDomainTest
    {
        [Fact]
        public void Create_InterviewExam_With_Valid_Parameters()
        {
            // Arrange
            InterviewExam interviewExam = new();

            // Act
            Action action = () =>
            {
                IEnumerable<Guid> questionIds = new List<Guid>();
                interviewExam.CreateExam(Guid.NewGuid(), "Test Title", "Test Description", questionIds);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_InterviewExam_With_Empty_CompanyId_Should_Throw()
        {
            // Arrange
            InterviewExam interviewExam = new();

            // Act
            Action action = () =>
            {
                interviewExam.CreateExam(Guid.Empty, "Test Title", "Test Description", new List<Guid>());
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_InterviewExam_With_Empty_QuestionId_Should_Throw()
        {
            // Arrange
            InterviewExam interviewExam = new();

            // Act
            Action action = () =>
            {
                interviewExam.CreateExam(Guid.NewGuid(), "Test Title", "Test Description", new List<Guid> { Guid.Empty });
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
