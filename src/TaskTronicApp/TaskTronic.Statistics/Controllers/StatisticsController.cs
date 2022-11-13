namespace TaskTronic.Statistics.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.Statistics;
    using Services.Statistics;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;

    public class StatisticsController : ApiController
    {
        private readonly IStatisticsService statistics;

        public StatisticsController(IStatisticsService statistics)
            => this.statistics = statistics;

        [HttpGet]
        public async Task<OutputStatisticsServiceModel> Full()
            => await this.statistics.FullStatsAsync();
    }
}
