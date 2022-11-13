namespace TaskTronic.Identity.Services.Messages
{
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;
    using TaskTronic.Identity.Data;

    public class MessageService : IMessageService
    {
        private readonly IdentityDbContext dbContext;

        public MessageService(IdentityDbContext dbContext)
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
