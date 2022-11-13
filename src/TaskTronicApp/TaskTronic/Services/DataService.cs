namespace TaskTronic.Services
{
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class DataService<TEntity> : IDataService<TEntity>
        where TEntity : class
    {
        protected DataService(DbContext dbContext) 
            => this.Data = dbContext;

        protected DbContext Data { get; }

        protected IQueryable<TEntity> All() => this.Data.Set<TEntity>();

        public async Task Save(TEntity entity, params Message[] messages)
        {
            foreach (var message in messages)
            {
                this.Data.Add(message);
            }

            this.Data.Update(entity);

            await this.Data.SaveChangesAsync();
        }

        public async Task MarkMessageAsPublished(int id)
        {
            var message = await this.Data.FindAsync<Message>(id);

            message.MarkedAsPublished();

            await this.Data.SaveChangesAsync();
        }
    }
}
