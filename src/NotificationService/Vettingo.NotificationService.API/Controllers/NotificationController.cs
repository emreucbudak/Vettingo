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
            [FromQuery] bool unreadOnly = false)
        {
            return Ok(await mediator.Send(new GetUserNotificationsQueryRequest(userId, unreadOnly)));
        }

        [HttpGet("user/{userId:guid}/unread")]
        public async Task<IActionResult> GetUnreadUserNotifications([FromRoute] Guid userId)
        {
            return Ok(await mediator.Send(new GetUserNotificationsQueryRequest(userId, true)));
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
