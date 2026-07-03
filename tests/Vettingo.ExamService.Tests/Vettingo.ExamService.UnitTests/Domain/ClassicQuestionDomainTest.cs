using FluentAssertions;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ClassicQuestionDomainTest
    {
        [Fact]
        public async Task Create_ClassicQuestion_With_Valid_Parameters()
        {
            // Arrange
            Vettingo.ExamService.Domain.Entities.ClassicQuestion classicQuestion = new Vettingo.ExamService.Domain.Entities.ClassicQuestion();
            // Act
            Action action = () =>
            {
                classicQuestion.CreateQuestion(Guid.NewGuid(), "Sample Question",15,1.6m,2,"Sample explanation","Sample Answer");

            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
