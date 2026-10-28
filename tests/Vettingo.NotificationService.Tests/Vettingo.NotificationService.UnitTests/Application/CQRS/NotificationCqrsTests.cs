using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser;
using Vettingo.NotificationService.Application.Repository;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Domain.Entities;

namespace Vettingo.NotificationService.UnitTests.Application.CQRS
{
    public class NotificationCqrsTests
    {
        [Fact]
        public async Task CreateNotificationCommandHandler_Should_Save_And_Send()
        {
            var repository = Substitute.For<INotificationRepository>();
            var sender = Substitute.For<INotificationSender>();
            repository.AddNotificationAsync(Arg.Any<Notification>()).Returns(_ => CompleteAsync());
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            sender.SendNotificationAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>()).Returns(_ => CompleteAsync());
            var handler = new CreateNotificationCommandHandler(repository, sender, Substitute.For<ILogger<CreateNotificationCommandHandler>>());
            var request = new CreateNotificationCommandRequest
            {
                Id = Guid.CreateVersion7(),
                UserId = Guid.NewGuid(),
                Title = "Welcome",
                Message = "Welcome to Vettingo"
            };

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddNotificationAsync(Arg.Is<Notification>(notification =>
                notification.Id == request.Id &&
                notification.UserId == request.UserId &&
                notification.Title == request.Title &&
                notification.Message == request.Message));
            await repository.Received(1).SaveChangesAsync();
            await sender.Received(1).SendNotificationAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetUserNotificationsQueryHandler_Should_Return_All_Notification_Properties()
        {
            var repository = Substitute.For<INotificationRepository>();
            var userId = Guid.NewGuid();
            var notification = CreateNotification(userId, "Read");
            notification.MarkAsRead();
            repository.GetNotificationsByUserIdAsync(userId).Returns(_ => ReturnAsync<IEnumerable<Notification>>([notification]));
            var handler = new GetUserNotificationsQueryHandler(repository, Substitute.For<ILogger<GetUserNotificationsQueryHandler>>());

            var response = (await handler.Handle(new GetUserNotificationsQueryRequest(userId), CancellationToken.None)).ToList();

            response.Should().ContainSingle();
            response[0].Should().BeEquivalentTo(notification);
            await repository.Received(1).GetNotificationsByUserIdAsync(userId);
            await repository.DidNotReceiveWithAnyArgs().MarkNotificationsAsReadAsync(default, default!, default);
        }

        [Fact]
        public async Task GetUserNotificationsQueryHandler_Should_Update_Only_Fetched_Unread_Notifications_Before_Returning_Snapshot()
        {
            var repository = Substitute.For<INotificationRepository>();
            var userId = Guid.NewGuid();
            var unread = CreateNotification(userId, "Unread");
            var read = CreateNotification(userId, "Read");
            read.MarkAsRead();
            repository.GetNotificationsByUserIdAsync(userId).Returns(Task.FromResult<IEnumerable<Notification>>([unread, read]));
            var update = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            repository.MarkNotificationsAsReadAsync(userId, Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(update.Task);
            var handler = new GetUserNotificationsQueryHandler(repository, Substitute.For<ILogger<GetUserNotificationsQueryHandler>>());
            using var cancellation = new CancellationTokenSource();

            var responseTask = handler.Handle(new GetUserNotificationsQueryRequest(userId), cancellation.Token);

            responseTask.IsCompleted.Should().BeFalse();
            update.SetResult();
            var response = (await responseTask).ToList();

            await repository.Received(1).MarkNotificationsAsReadAsync(userId,
                Arg.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(unread.Id)), cancellation.Token);
            response.Should().HaveCount(2);
            response[0].Should().BeEquivalentTo(unread);
            response[0].IsRead.Should().BeFalse();
            response[1].Should().BeEquivalentTo(read);
            await repository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task GetUserNotificationsQueryHandler_Should_Not_Update_An_Empty_List()
        {
            var repository = Substitute.For<INotificationRepository>();
            var userId = Guid.NewGuid();
            repository.GetNotificationsByUserIdAsync(userId).Returns(Task.FromResult<IEnumerable<Notification>>([]));
            var handler = new GetUserNotificationsQueryHandler(repository, Substitute.For<ILogger<GetUserNotificationsQueryHandler>>());

            var response = await handler.Handle(new GetUserNotificationsQueryRequest(userId), CancellationToken.None);

            response.Should().BeEmpty();
            await repository.DidNotReceiveWithAnyArgs().MarkNotificationsAsReadAsync(default, default!, default);
        }

        [Fact]
        public async Task GetUserNotificationsQueryHandler_Should_Propagate_Update_Failure()
        {
            var repository = Substitute.For<INotificationRepository>();
            var userId = Guid.NewGuid();
            repository.GetNotificationsByUserIdAsync(userId)
                .Returns(Task.FromResult<IEnumerable<Notification>>([CreateNotification(userId, "Unread")]));
            repository.MarkNotificationsAsReadAsync(userId, Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new InvalidOperationException("Update failed")));
            var handler = new GetUserNotificationsQueryHandler(repository, Substitute.For<ILogger<GetUserNotificationsQueryHandler>>());

            Func<Task> handle = () => handler.Handle(new GetUserNotificationsQueryRequest(userId), CancellationToken.None);

            await handle.Should().ThrowAsync<InvalidOperationException>().WithMessage("Update failed");
        }

        private static Notification CreateNotification(Guid userId, string title)
        {
            Notification notification = new();
            notification.CreateNotification(userId, title, "Notification message");
            return notification;
        }

        private static async Task CompleteAsync()
        {
            await Task.Yield();
        }

        private static async Task<T> ReturnAsync<T>(T value)
        {
            await Task.Yield();
            return value;
        }
    }
}
