namespace TaskTronic.Statistics.Messages
{
    using MassTransit;
    using Services.Statistics;
    using System.Threading.Tasks;
    using TaskTronic.Messages.Drive.Files;

    public class FileDeletedConsumer : IConsumer<FileDeletedMessage>
    {
        private readonly IStatisticsService statisticsService;

        public FileDeletedConsumer(IStatisticsService statisticsService)
            => this.statisticsService = statisticsService;

        public async Task Consume(ConsumeContext<FileDeletedMessage> context)
            => await this.statisticsService.RemoveFileAsync();
    }
}
