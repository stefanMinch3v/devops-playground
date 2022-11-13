namespace TaskTronic.Statistics.Messages
{
    using MassTransit;
    using Services.Statistics;
    using System.Threading.Tasks;
    using TaskTronic.Messages.Drive.Files;

    public class FileUploadedConsumer : IConsumer<FileUploadedMessage>
    {
        private readonly IStatisticsService statisticsService;

        public FileUploadedConsumer(IStatisticsService statisticsService)
            => this.statisticsService = statisticsService;

        public async Task Consume(ConsumeContext<FileUploadedMessage> context)
            => await this.statisticsService.AddFileAsync();
    }
}
