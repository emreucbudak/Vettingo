using FluentAssertions;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Domain.Enums;

namespace Vettingo.NotificationService.UnitTests.Domain
{
    public class NotificationDomainTest
    {
        [Fact]
        public void Create_Notification_With_Valid_Parameters()
        {
            // Arrange
            Notification notification = new();

            // Act
            Action action = () =>
            {
                notification.CreateNotification(Guid.NewGuid(), "Test Title", "Test Message", NotificationType.Info);
            };

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Create_Notification_With_Empty_UserId_Should_Throw()
        {
            // Arrange
            Notification notification = new();

            // Act
            Action action = () =>
            {
                notification.CreateNotification(Guid.Empty, "Test Title", "Test Message", NotificationType.Info);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_Notification_With_Empty_Title_Should_Throw()
        {
            // Arrange
            Notification notification = new();

            // Act
            Action action = () =>
            {
                notification.CreateNotification(Guid.NewGuid(), string.Empty, "Test Message", NotificationType.Info);
            };

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
