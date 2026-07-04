using FluentAssertions;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.UnitTests.Domain
{
    public class ExamAttemptDomainTest
    {
        [Fact]
        public void Start_ExamAttempt_With_Valid_Parameters()
        {
            // Arrange
            var examId = Guid.NewGuid();
            var candidateId = Guid.NewGuid();

            // Act
            Action action = () =>
            {
                var attempt = new ExamAttempt();
                attempt.StartAttempt(examId, candidateId);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Start_ExamAttempt_With_Empty_ExamId_Should_Throw()
        {
            // Arrange
            var candidateId = Guid.NewGuid();

            // Act
            Action action = () =>
            {
                var attempt = new ExamAttempt();
                attempt.StartAttempt(Guid.Empty, candidateId);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Complete_ExamAttempt_With_Invalid_Score_Should_Throw()
        {
            // Arrange
            var attempt = new ExamAttempt();
            attempt.StartAttempt(Guid.NewGuid(), Guid.NewGuid());

            // Act
            Action action = () => attempt.CompleteAttempt(101m);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
