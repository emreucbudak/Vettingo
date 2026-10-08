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
                    "Test Description",
                    "Test Requirements",
                    "Test Responsibilities",
                    34,
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    ExperienceLevel.Lead,
                    50000,
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
                    "Test Description",
                    "Test Requirements",
                    "Test Responsibilities",
                    34,
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    ExperienceLevel.Lead,
                    50000,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Active);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_JobPosting_With_Negative_Salary_Should_Throw()
        {
            // Arrange
            JobPosting jobPosting = new();

            // Act
            Action action = () =>
            {
                jobPosting.CreateJobPosting(
                    Guid.NewGuid(),
                    "Test Title",
                    "Test Description",
                    "Test Requirements",
                    "Test Responsibilities",
                    34,
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    ExperienceLevel.Lead,
                    -1,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Active);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
