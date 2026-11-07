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
                    JobPostingStatus.AcceptingApplications, [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
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
                2, 5, 50000, 75000, null, JobPostingStatus.AcceptingApplications, [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
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
                    JobPostingStatus.AcceptingApplications, [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Create_JobPosting_Without_JobSteps_Should_Throw(int invalidCase)
        {
            var posting = new JobPosting();
            List<JobSteps> steps = invalidCase switch { 0 => null!, 1 => [], _ => [null!] };

            Action create = () => posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", "Developer",
                new Dictionary<string, int> { ["C#"] = 3 }, 34, EmploymentType.FullTime, WorkingModel.Remote,
                2, 5, 50000, 75000, null, JobPostingStatus.AcceptingApplications, steps);

            create.Should().Throw<ArgumentException>().WithParameterName("jobSteps");
            posting.Id.Should().BeEmpty();
            posting.JobSteps.Should().BeEmpty();
        }

        [Fact]
        public void Create_JobPosting_Should_Use_Provided_JobSteps_List()
        {
            var posting = new JobPosting();
            var step = new JobSteps
            {
                Order = 1, StepType = StepType.Exam,
                StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2)
            };
            var steps = new List<JobSteps> { step };

            posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", "Developer",
                new Dictionary<string, int> { ["C#"] = 3 }, 34, EmploymentType.FullTime, WorkingModel.Remote,
                2, 5, 50000, 75000, null, JobPostingStatus.AcceptingApplications, steps);
            posting.JobSteps.Should().ContainSingle().Which.Should().BeSameAs(step);
            posting.IsApplicationClosed.Should().BeFalse();
            posting.IsPostingClosed.Should().BeFalse();
            ((ICollection<JobSteps>)posting.JobSteps).IsReadOnly.Should().BeTrue();

            steps.Clear();
            posting.JobSteps.Should().BeEmpty();
        }
    }
}
