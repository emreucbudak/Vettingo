using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser;

namespace Vettingo.NotificationService.API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize(Roles = "Company,Candidate")]
    public class NotificationController(IMediator mediator) : ControllerBase
    {
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserNotifications([FromRoute] Guid userId)
        {
            return Ok(await mediator.Send(new GetUserNotificationsQueryRequest(userId)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }
    }
}
