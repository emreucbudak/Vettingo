using FluentAssertions;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewExamDomainTest
    {
        [Fact]
        public async Task Create_InterviewExam_With_Valid_Parameters()
        {
            // Arrange
            InterviewService.Domain.Entities.InterviewExam interviewExam = new InterviewService.Domain.Entities.InterviewExam();
            // Act
            Action action = () =>
            {
                IEnumerable<Guid> questionIds = new List<Guid>();
                interviewExam.CreateExam(Guid.NewGuid(),"Test Title", "Test Description", questionIds);
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
