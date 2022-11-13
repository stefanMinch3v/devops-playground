namespace TaskTronic.Drive.Services.Messages
{
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;
    using TaskTronic.Drive.Data;

    public class MessageService : IMessageService
    {
        private readonly DriveDbContext dbContext;

        public MessageService(DriveDbContext dbContext) 
            => this.dbContext = dbContext;

        public async Task MarkMessageAsPublishedAsync(int id)
        {
            var message = await this.dbContext.Messages.FindAsync(id);

            if (message is null)
            {
                return;
            }

            message.MarkedAsPublished();

            await this.dbContext.SaveChangesAsync();
        }

        public async Task SaveAsync(Message message)
        {
            this.dbContext.Messages.Add(message);

            await this.dbContext.SaveChangesAsync();
        }
    }
}
