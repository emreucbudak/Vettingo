using Microsoft.AspNetCore.SignalR;

namespace Vettingo.NotificationService.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public Task JoinUserGroup(Guid userId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        }

        public Task LeaveUserGroup(Guid userId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        }
    }
}
