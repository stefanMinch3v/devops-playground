namespace TaskTronic.Admin.Services.Statistics
{
    using Models.Statistics;
    using Refit;
    using System.Threading.Tasks;

    public interface IStatisticsService
    {
        [Get("/Statistics")]
        Task<OutputStatisticsServiceModel> Full();
    }
}
