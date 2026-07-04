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
                    "Test Location",
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    ExperienceLevel.Lead,
                    50000m,
                    70000m,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Published);
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
                    "Test Location",
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    ExperienceLevel.Lead,
                    50000m,
                    70000m,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Published);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_JobPosting_With_MinSalary_Greater_Than_MaxSalary_Should_Throw()
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
                    "Test Location",
                    EmploymentType.FullTime,
                    WorkingModel.OnSite,
                    ExperienceLevel.Lead,
                    80000m,
                    70000m,
                    DateTime.UtcNow.AddDays(30),
                    JobPostingStatus.Published);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
