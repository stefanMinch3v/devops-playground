namespace TaskTronic.Notifications.Messages
{
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using TaskTronic.Messages.Drive.Folders;
    using TaskTronic.Notifications.Hubs;

    public class FolderCreatedConsumer : IConsumer<FolderCreatedMessage>
    {
        private readonly IHubContext<NotificationsHub> hub;

        public FolderCreatedConsumer(IHubContext<NotificationsHub> hub) 
            => this.hub = hub;

        public async Task Consume(ConsumeContext<FolderCreatedMessage> context)
            => await this.hub
                .Clients
                .Groups(NotificationConstants.AuthenticatedUsersGroup)
                .SendAsync(NotificationConstants.ReceiveNotificationEndpoint, context.Message);
    }
}
