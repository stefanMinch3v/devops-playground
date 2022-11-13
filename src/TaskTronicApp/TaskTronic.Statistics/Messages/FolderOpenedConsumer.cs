namespace TaskTronic.Statistics.Messages
{
    using MassTransit;
    using Services.FolderViews;
    using System.Threading.Tasks;
    using TaskTronic.Messages.Drive.Folders;

    public class FolderOpenedConsumer : IConsumer<FolderOpenedMessage>
    {
        private readonly IFolderViewService folderViewService;

        public FolderOpenedConsumer(IFolderViewService folderViewService) 
            => this.folderViewService = folderViewService;

        public async Task Consume(ConsumeContext<FolderOpenedMessage> context)
        {
            var message = context.Message;
            await this.folderViewService.AddViewAsync(message.FolderId, message.UserId);
        }
    }
}
