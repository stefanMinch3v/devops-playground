namespace TaskTronic.Identity.Services.Messages
{
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;

    public interface IMessageService
    {
        Task MarkMessageAsPublishedAsync(int id);

        Task SaveAsync(Message message);
    }
}
