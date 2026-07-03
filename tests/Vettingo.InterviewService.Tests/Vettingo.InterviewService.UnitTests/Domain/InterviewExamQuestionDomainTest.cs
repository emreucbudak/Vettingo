using FluentAssertions;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewExamQuestionDomainTest
    {
        [Fact]
        public async Task Create_InterviewExamQuestion_With_Valid_Parameters()
        {
            // Arrange
            InterviewService.Domain.Entities.InterviewExamQuestion interviewExamQuestion = new InterviewService.Domain.Entities.InterviewExamQuestion();
            // Act
            Action action = () =>
            {
                interviewExamQuestion.CreateExamQuestion(Guid.NewGuid(), Guid.NewGuid(), 3);
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
