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
                Id = Guid.CreateVersion7(),
                UserId = Guid.NewGuid(),
                Title = "Welcome",
                Message = "Welcome to Vettingo"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Id.Should().Be(request.Id);
            response.UserId.Should().Be(request.UserId);
            response.Title.Should().Be(request.Title);
            response.Message.Should().Be(request.Message);
            response.IsRead.Should().BeFalse();
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
