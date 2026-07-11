using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.NotificationService.Application.Exceptions;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.MarkNotificationAsRead;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser;
using Vettingo.NotificationService.Application.Repository;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Domain.Enums;

namespace Vettingo.NotificationService.UnitTests.Application.CQRS
{
    public class NotificationCqrsTests
    {
        [Fact]
        public async Task CreateNotificationCommandHandler_Should_Save_Send_And_Return_Response()
        {
            var repository = Substitute.For<INotificationRepository>();
            var sender = Substitute.For<INotificationSender>();
            repository.AddNotificationAsync(Arg.Any<Notification>()).Returns(_ => CompleteAsync());
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            sender.SendNotificationAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>()).Returns(_ => CompleteAsync());
            var handler = new CreateNotificationCommandHandler(repository, sender, Substitute.For<ILogger<CreateNotificationCommandHandler>>());
            var request = new CreateNotificationCommandRequest
            {
                UserId = Guid.NewGuid(),
                Title = "Welcome",
                Message = "Welcome to Vettingo",
                Type = NotificationType.Success
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.UserId.Should().Be(request.UserId);
            response.Title.Should().Be(request.Title);
            response.Message.Should().Be(request.Message);
            response.Type.Should().Be(request.Type);
            response.IsRead.Should().BeFalse();
            await repository.Received(1).AddNotificationAsync(Arg.Is<Notification>(notification =>
                notification.UserId == request.UserId &&
                notification.Title == request.Title &&
                notification.Message == request.Message));
            await repository.Received(1).SaveChangesAsync();
            await sender.Received(1).SendNotificationAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MarkNotificationAsReadCommandHandler_When_Notification_Not_Found_Should_Throw_NotFoundException()
        {
            var repository = Substitute.For<INotificationRepository>();
            var notificationId = Guid.NewGuid();
            repository.GetNotificationByIdAsync(notificationId).Returns(_ => ReturnAsync<Notification?>(null));
            var handler = new MarkNotificationAsReadCommandHandler(repository, Substitute.For<ILogger<MarkNotificationAsReadCommandHandler>>());

            Func<Task> action = () => handler.Handle(new MarkNotificationAsReadCommandRequest { NotificationId = notificationId }, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            repository.DidNotReceive().UpdateNotification(Arg.Any<Notification>());
            await repository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task MarkNotificationAsReadCommandHandler_Should_Mark_Notification_As_Read()
        {
            var repository = Substitute.For<INotificationRepository>();
            var notification = CreateNotification(Guid.NewGuid(), "Reminder");
            repository.GetNotificationByIdAsync(notification.Id).Returns(_ => ReturnAsync<Notification?>(notification));
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new MarkNotificationAsReadCommandHandler(repository, Substitute.For<ILogger<MarkNotificationAsReadCommandHandler>>());

            await handler.Handle(new MarkNotificationAsReadCommandRequest { NotificationId = notification.Id }, CancellationToken.None);

            notification.IsRead.Should().BeTrue();
            notification.ReadAt.Should().NotBeNull();
            repository.Received(1).UpdateNotification(notification);
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetUserNotificationsQueryHandler_When_UnreadOnly_Should_Use_Unread_Repository_Method()
        {
            var repository = Substitute.For<INotificationRepository>();
            var userId = Guid.NewGuid();
            var notification = CreateNotification(userId, "Unread");
            repository.GetUnreadNotificationsByUserIdAsync(userId).Returns(_ => ReturnAsync<IEnumerable<Notification>>([notification]));
            var handler = new GetUserNotificationsQueryHandler(repository, Substitute.For<ILogger<GetUserNotificationsQueryHandler>>());

            var response = (await handler.Handle(new GetUserNotificationsQueryRequest(userId, true), CancellationToken.None)).ToList();

            response.Should().ContainSingle();
            response[0].Id.Should().Be(notification.Id);
            await repository.Received(1).GetUnreadNotificationsByUserIdAsync(userId);
            await repository.DidNotReceive().GetNotificationsByUserIdAsync(Arg.Any<Guid>());
        }

        private static Notification CreateNotification(Guid userId, string title)
        {
            Notification notification = new();
            notification.CreateNotification(userId, title, "Notification message", NotificationType.Info);
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
