using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.MarkNotificationAsRead;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser;

namespace Vettingo.NotificationService.API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController(IMediator mediator) : ControllerBase
    {
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserNotifications(
            [FromRoute] Guid userId,
            [FromQuery] GetUserNotificationsQueryRequest request)
        {
            return Ok(await mediator.Send(new GetUserNotificationsQueryRequest
            {
                UserId = userId,
                UnreadOnly = request.UnreadOnly
            }));
        }

        [HttpGet("user/{userId:guid}/unread")]
        public async Task<IActionResult> GetUnreadUserNotifications(
            [FromRoute] Guid userId,
            [FromQuery] GetUserNotificationsQueryRequest request)
        {
            return Ok(await mediator.Send(new GetUserNotificationsQueryRequest { UserId = userId, UnreadOnly = true }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPut("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkNotificationAsRead([FromRoute] Guid notificationId)
        {
            await mediator.Send(new MarkNotificationAsReadCommandRequest { NotificationId = notificationId });
            return Ok();
        }
    }
}
