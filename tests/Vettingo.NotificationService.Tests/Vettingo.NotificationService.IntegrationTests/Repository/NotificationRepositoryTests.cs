using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Domain.Enums;
using Vettingo.NotificationService.Persistence.DbContext;
using Vettingo.NotificationService.Persistence.Repository;

namespace Vettingo.NotificationService.UnitTests.Repository
{
    public class NotificationRepositoryTests
    {
        [Fact]
        public async Task AddNotificationAsync_Then_GetNotificationByIdAsync_Should_Return_Notification()
        {
            await using var context = CreateContext();
            var repository = new NotificationRepository(context);
            var notification = CreateNotification(Guid.NewGuid(), "Welcome");

            await repository.AddNotificationAsync(notification);
            await repository.SaveChangesAsync();

            var result = await repository.GetNotificationByIdAsync(notification.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(notification.Id);
            result.Title.Should().Be("Welcome");
        }

        [Fact]
        public async Task GetNotificationsByUserIdAsync_Should_Return_Only_User_Notifications()
        {
            await using var context = CreateContext();
            var repository = new NotificationRepository(context);
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            await repository.AddNotificationAsync(CreateNotification(userId, "First"));
            await repository.AddNotificationAsync(CreateNotification(userId, "Second"));
            await repository.AddNotificationAsync(CreateNotification(otherUserId, "Other"));
            await repository.SaveChangesAsync();

            var result = (await repository.GetNotificationsByUserIdAsync(userId)).ToList();

            result.Should().HaveCount(2);
            result.Should().OnlyContain(notification => notification.UserId == userId);
        }

        [Fact]
        public async Task GetUnreadNotificationsByUserIdAsync_Should_Exclude_Read_Notifications()
        {
            await using var context = CreateContext();
            var repository = new NotificationRepository(context);
            var userId = Guid.NewGuid();
            var unreadNotification = CreateNotification(userId, "Unread");
            var readNotification = CreateNotification(userId, "Read");
            readNotification.MarkAsRead();

            await repository.AddNotificationAsync(unreadNotification);
            await repository.AddNotificationAsync(readNotification);
            await repository.SaveChangesAsync();

            var result = (await repository.GetUnreadNotificationsByUserIdAsync(userId)).ToList();

            result.Should().ContainSingle();
            result[0].Id.Should().Be(unreadNotification.Id);
            result[0].IsRead.Should().BeFalse();
        }

        private static NotificationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase($"notification-repository-{Guid.NewGuid()}")
                .Options;

            return new NotificationDbContext(options);
        }

        private static Notification CreateNotification(Guid userId, string title)
        {
            Notification notification = new();
            notification.CreateNotification(userId, title, "Notification message", NotificationType.Info);
            return notification;
        }
    }
}
