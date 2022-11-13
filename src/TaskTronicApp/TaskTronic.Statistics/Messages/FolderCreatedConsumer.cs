namespace TaskTronic.Statistics.Messages
{
    using MassTransit;
    using Services.Statistics;
    using System.Threading.Tasks;
    using TaskTronic.Messages.Drive.Folders;

    public class FolderCreatedConsumer : IConsumer<FolderCreatedMessage>
    {
        private readonly IStatisticsService statisticsService;

        public FolderCreatedConsumer(IStatisticsService statisticsService) 
            => this.statisticsService = statisticsService;

        public async Task Consume(ConsumeContext<FolderCreatedMessage> context)
            => await this.statisticsService.AddFolderAsync();
    }
}
