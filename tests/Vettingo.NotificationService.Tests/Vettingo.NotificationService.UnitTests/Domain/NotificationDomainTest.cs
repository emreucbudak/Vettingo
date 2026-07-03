using FluentAssertions;
using NSubstitute.ExceptionExtensions;
namespace Vettingo.NotificationService.UnitTests.Domain
{
    public class NotificationDomainTest
    {
        [Fact]
        public async Task Create_Notification_With_Valid_Parameters()
        {
            // Arrange
            NotificationService.Domain.Entities.Notification notification = new NotificationService.Domain.Entities.Notification();
            // Act
            Action action = () => {
                notification.CreateNotification(Guid.NewGuid(), "Test Title", "Test Message", NotificationService.Domain.Enums.NotificationType.Info);
            };
            // Assert
            action.Should().NotThrow();
        }
    }
}
