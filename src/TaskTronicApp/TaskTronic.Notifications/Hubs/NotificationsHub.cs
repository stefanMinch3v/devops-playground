namespace TaskTronic.Notifications.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Threading.Tasks;

    public class NotificationsHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            if (this.Context.User.Identity.IsAuthenticated)
            {
                await this.Groups.AddToGroupAsync(
                    this.Context.ConnectionId, 
                    NotificationConstants.AuthenticatedUsersGroup);
            }
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (this.Context.User.Identity.IsAuthenticated)
            {
                await this.Groups.RemoveFromGroupAsync(
                    this.Context.ConnectionId, 
                    NotificationConstants.AuthenticatedUsersGroup);
            }
        }
    }
}
