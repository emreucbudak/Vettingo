using FluentAssertions;

namespace Vettingo.JobService.UnitTests.Domain
{
    public class JobPostingDomainTest
    {
        [Fact]
        public async Task Create_JobPosting_With_Valid_Parameters()
        {
            // Arrange
            Vettingo.JobService.Domain.Entities.JobPosting jobPosting = new Vettingo.JobService.Domain.Entities.JobPosting();
            // Act
            Action action = () =>
            {
                jobPosting.CreateJobPosting(
                    Guid.NewGuid(),
                    "Test Title",
                    "Test Description",
                    "Test Requirements",
                    "Test Responsibilities",
                    "Test Location",
                    Vettingo.JobService.Domain.Enums.EmploymentType.FullTime,
                    Vettingo.JobService.Domain.Enums.WorkingModel.OnSite,
                    Vettingo.JobService.Domain.Enums.ExperienceLevel.Lead,
                    50000m,
                    70000m,
                    DateTime.UtcNow.AddDays(30),
                    Vettingo.JobService.Domain.Enums.JobPostingStatus.Published);
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
