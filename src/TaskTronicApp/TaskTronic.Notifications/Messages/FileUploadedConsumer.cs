namespace TaskTronic.Notifications.Messages
{
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using TaskTronic.Messages.Drive.Files;
    using TaskTronic.Notifications.Hubs;

    public class FileUploadedConsumer : IConsumer<FileUploadedMessage>
    {
        private readonly IHubContext<NotificationsHub> hub;

        public FileUploadedConsumer(IHubContext<NotificationsHub> hub) 
            => this.hub = hub;

        public async Task Consume(ConsumeContext<FileUploadedMessage> context)
            => await this.hub
                .Clients
                .Groups(NotificationConstants.AuthenticatedUsersGroup)
                .SendAsync(NotificationConstants.ReceiveNotificationEndpoint, context.Message);
    }
}
