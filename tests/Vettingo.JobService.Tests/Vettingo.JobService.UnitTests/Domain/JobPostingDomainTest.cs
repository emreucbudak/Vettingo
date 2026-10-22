using FluentAssertions;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Domain
{
    public class JobPostingDomainTest
    {
        [Fact]
        public void Create_JobPosting_With_Valid_Parameters()
        {
            // Arrange
            JobPosting jobPosting = new();

            // Act
            Action action = () =>
            {
                jobPosting.CreateJobPosting(
                    Guid.NewGuid(),
                    "Test Title",
                    new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
                    34,
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    2, 5,
                    50000, 50000,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Active);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_JobPosting_With_Empty_CompanyId_Should_Throw()
        {
            // Arrange
            JobPosting jobPosting = new();

            // Act
            Action action = () =>
            {
                jobPosting.CreateJobPosting(
                    Guid.Empty,
                    "Test Title",
                    new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
                    34,
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    2, 5,
                    50000, 50000,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Active);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

    }
}
