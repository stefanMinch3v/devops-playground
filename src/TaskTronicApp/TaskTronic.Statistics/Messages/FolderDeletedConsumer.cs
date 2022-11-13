namespace TaskTronic.Statistics.Messages
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Statistics;
    using TaskTronic.Messages.Drive.Folders;

    public class FolderDeletedConsumer : IConsumer<FolderDeletedMessage>
    {
        private readonly IStatisticsService statisticsService;

        public FolderDeletedConsumer(IStatisticsService statisticsService)
            => this.statisticsService = statisticsService;

        public async Task Consume(ConsumeContext<FolderDeletedMessage> context)
            => await this.statisticsService.RemoveFolderAsync();
    }
}
