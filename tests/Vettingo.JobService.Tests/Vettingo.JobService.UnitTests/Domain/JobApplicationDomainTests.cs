using FluentAssertions;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Domain
{
    public class JobApplicationDomainTests
    {
        [Fact]
        public void CreateApplication_WithValidData_ShouldCreateApplication()
        {
            JobApplication application = new();
            var appliedAt = DateTime.UtcNow;

            application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), appliedAt, ApplicationStatus.Başvuruldu);

            application.Id.Should().NotBeEmpty();
            application.AppliedAt.Should().Be(appliedAt);
            application.Status.Should().Be(ApplicationStatus.Başvuruldu);
        }

        [Fact]
        public void CreateApplication_WithEmptyJobPostingId_ShouldThrow()
        {
            JobApplication application = new();

            Action action = () => application.CreateApplication(
                Guid.NewGuid(), Guid.Empty, DateTime.UtcNow, ApplicationStatus.Başvuruldu);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateStatus_ShouldChangeStatusAndUpdatedAt()
        {
            JobApplication application = new();
            application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.Başvuruldu);

            application.UpdateStatus(ApplicationStatus.Mülakat);

            application.Status.Should().Be(ApplicationStatus.Mülakat);
            application.UpdatedAt.Should().NotBeNull();
        }
    }
}
