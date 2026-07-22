using FluentAssertions;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Domain.Enums;

namespace Vettingo.InterviewService.UnitTests.Domain
{
    public class InterviewExamDomainTest
    {
        [Fact]
        public void Create_Human_Interview_With_Valid_Parameters()
        {
            InterviewExam interviewExam = new();
            var startDate = DateTime.UtcNow.AddDays(1);

            Action action = () => interviewExam.CreateExam(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Test Title",
                "Test Description",
                InterviewType.Human,
                startDate,
                null,
                []);

            action.Should().NotThrow();
            interviewExam.Type.Should().Be(InterviewType.Human);
            interviewExam.StartDate.Should().Be(startDate);
            interviewExam.EndDate.Should().BeNull();
        }

        [Fact]
        public void Create_Interview_With_Empty_CompanyId_Should_Throw()
        {
            InterviewExam interviewExam = new();

            Action action = () => interviewExam.CreateExam(
                Guid.Empty,
                Guid.NewGuid(),
                "Test Title",
                "Test Description",
                InterviewType.Human,
                DateTime.UtcNow.AddDays(1),
                null,
                []);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_Interview_With_Empty_QuestionId_Should_Throw()
        {
            InterviewExam interviewExam = new();

            Action action = () => interviewExam.CreateExam(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Test Title",
                "Test Description",
                InterviewType.Human,
                DateTime.UtcNow.AddDays(1),
                null,
                [Guid.Empty]);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_AI_Interview_Without_EndDate_Should_Throw()
        {
            InterviewExam interviewExam = new();

            Action action = () => interviewExam.CreateExam(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "AI Interview",
                "AI interview description",
                InterviewType.AI,
                DateTime.UtcNow.AddDays(1),
                null,
                [Guid.NewGuid()]);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_AI_Interview_With_EndDate_Before_StartDate_Should_Throw()
        {
            InterviewExam interviewExam = new();
            var startDate = DateTime.UtcNow.AddDays(2);

            Action action = () => interviewExam.CreateExam(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "AI Interview",
                "AI interview description",
                InterviewType.AI,
                startDate,
                startDate.AddMinutes(-1),
                [Guid.NewGuid()]);

            action.Should().Throw<ArgumentException>();
        }
    }
}
