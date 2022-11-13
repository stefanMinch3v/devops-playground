namespace TaskTronic.Statistics.Services.Statistics
{
    using Models.Statistics;
    using System.Threading.Tasks;

    public interface IStatisticsService
    {
        Task<OutputStatisticsServiceModel> FullStatsAsync();

        Task AddFolderAsync();

        Task AddFileAsync();

        Task RemoveFileAsync();

        Task RemoveFolderAsync();
    }
}
