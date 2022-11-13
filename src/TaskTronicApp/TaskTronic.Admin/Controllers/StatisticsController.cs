namespace TaskTronic.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Services.Statistics;
    using System.Threading.Tasks;

    public class StatisticsController : AdministrationController
    {
        private readonly IStatisticsService statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
            => this.statisticsService = statisticsService;

        public async Task<IActionResult> Index()
            => View(await this.statisticsService.Full());
    }
}
