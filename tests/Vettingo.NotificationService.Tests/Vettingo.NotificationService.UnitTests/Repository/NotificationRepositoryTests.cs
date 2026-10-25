using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Persistence.DbContext;
using Vettingo.NotificationService.Persistence.Repository;

namespace Vettingo.NotificationService.UnitTests.Repository;

public class NotificationRepositoryTests
{
    [Fact]
    public async Task GetNotificationsByUserIdAsync_Should_Return_Latest_80_User_Notifications_In_Descending_Order()
    {
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new NotificationDbContext(options);
        var repository = new NotificationRepository(context);
        var userId = Guid.NewGuid();
        var notifications = new List<Notification>();
        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        for (var index = 0; index < 85; index++)
        {
            Notification notification = new();
            notification.CreateNotification(userId, $"Notification {index}", "Message");
            if (index % 2 == 0)
            {
                notification.MarkAsRead();
            }

            context.Notifications.Add(notification);
            context.Entry(notification).Property(item => item.CreatedAt).CurrentValue = createdAt.AddMinutes(index);
            notifications.Add(notification);
        }

        Notification otherUserNotification = new();
        otherUserNotification.CreateNotification(Guid.NewGuid(), "Other user", "Message");
        context.Notifications.Add(otherUserNotification);
        await context.SaveChangesAsync();

        var result = (await repository.GetNotificationsByUserIdAsync(userId)).ToList();

        result.Should().HaveCount(80);
        result.Should().OnlyContain(notification => notification.UserId == userId);
        result.Select(notification => notification.Id).Should().Equal(
            notifications.Skip(5).Reverse().Select(notification => notification.Id));
        result.Should().Contain(notification => notification.IsRead);
        result.Should().Contain(notification => !notification.IsRead);
    }
}
