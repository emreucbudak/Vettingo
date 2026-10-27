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
                    "Vettingo Teknoloji",
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
            jobPosting.CompanyName.Should().Be("Vettingo Teknoloji");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_JobPosting_With_Blank_CompanyName_Should_Throw(string? companyName)
        {
            var posting = new JobPosting();
            Action create = () => posting.CreateJobPosting(Guid.NewGuid(), companyName!, "Developer",
                new Dictionary<string, int> { ["C#"] = 3 }, 34, EmploymentType.FullTime, WorkingModel.Remote,
                2, 5, 50000, 75000, null, JobPostingStatus.Active);
            create.Should().Throw<ArgumentException>().WithParameterName("companyName");
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
                    "Vettingo Teknoloji",
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
