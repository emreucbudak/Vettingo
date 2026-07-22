using FluentAssertions;
using Vettingo.ApplicationService.Domain.Entities;
using Vettingo.ApplicationService.Domain.Enums;

namespace Vettingo.ApplicationService.UnitTests.Domain
{
    public class JobApplicationDomainTests
    {
        [Fact]
        public void CreateApplication_WithValidData_ShouldCreateApplication()
        {
            JobApplication application = new();
            var appliedAt = DateTime.UtcNow;

            application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), appliedAt, ApplicationStatus.Submitted);

            application.Id.Should().NotBeEmpty();
            application.AppliedAt.Should().Be(appliedAt);
            application.Status.Should().Be(ApplicationStatus.Submitted);
        }

        [Fact]
        public void CreateApplication_WithEmptyJobPostingId_ShouldThrow()
        {
            JobApplication application = new();

            Action action = () => application.CreateApplication(
                Guid.NewGuid(), Guid.Empty, DateTime.UtcNow, ApplicationStatus.Submitted);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateStatus_ShouldChangeStatusAndUpdatedAt()
        {
            JobApplication application = new();
            application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.Submitted);

            application.UpdateStatus(ApplicationStatus.Interview);

            application.Status.Should().Be(ApplicationStatus.Interview);
            application.UpdatedAt.Should().NotBeNull();
        }
    }
}
