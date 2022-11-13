namespace TaskTronic.Messages
{
    using Hangfire;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;

    // for background and long running tasks
    public class MessagesHostedService : IHostedService
    {
        private readonly IRecurringJobManager recurringJobManager;
        private readonly IBus publisher;
        private readonly IServiceProvider services;

        public MessagesHostedService(
            IRecurringJobManager recurringJobManager,
            IBus publisher,
            IServiceProvider services)
        {
            this.recurringJobManager = recurringJobManager;
            this.publisher = publisher;
            this.services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = this.services.CreateScope();

            var dbContext = scope.ServiceProvider
                    .GetRequiredService<DbContext>();

            if (!dbContext.Database.CanConnect())
            {
                dbContext.Database.Migrate();
            }

            this.recurringJobManager.AddOrUpdate(
                nameof(MessagesHostedService),
                () => this.ProccessPendingMessages(cancellationToken),
                "*/15 * * * * *"); // Cron job expression syntax -> runs in every 15 sec

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        // must be public and not async in order to work with recurring job manager
        // hangfire will handle all failures if something went wrong with the method below
        public void ProccessPendingMessages(CancellationToken cancellationToken)
        {
            using var scope = this.services.CreateScope();

            var dbContext = scope.ServiceProvider
                    .GetRequiredService<DbContext>();

            var messages = dbContext
                .Set<Message>()
                .Where(m => !m.Published)
                .OrderBy(m => m.Id)
                .ToList();

            foreach (var message in messages)
            {
                this.publisher.Publish(message.Data, message.Type, cancellationToken)
                    .GetAwaiter()
                    .GetResult();

                message.MarkedAsPublished();

                dbContext.SaveChanges();
            }
        }
    }
}
