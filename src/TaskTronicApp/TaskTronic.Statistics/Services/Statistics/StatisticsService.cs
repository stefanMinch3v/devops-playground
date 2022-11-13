namespace TaskTronic.Statistics.Services.Statistics
{
    using AutoMapper;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Statistics;
    using System.Threading.Tasks;
    using TaskTronic.Services;

    public class StatisticsService : IStatisticsService
    {
        private readonly IMapper mapper;
        private readonly StatisticsDbContext dbContext;

        public StatisticsService(StatisticsDbContext dbContext, IMapper mapper)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task AddFileAsync()
        {
            var statistics = await this.dbContext.Statistics.SingleOrDefaultAsync();

            statistics.TotalFiles++;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task AddFolderAsync()
        {
            var statistics = await this.dbContext.Statistics.SingleOrDefaultAsync();

            statistics.TotalFolders++;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<OutputStatisticsServiceModel> FullStatsAsync()
            => await this.mapper
                .ProjectTo<OutputStatisticsServiceModel>(this.dbContext.Statistics)
                .FirstOrDefaultAsync();

        public async Task RemoveFileAsync()
        {
            var statistics = await this.dbContext.Statistics.SingleOrDefaultAsync();

            statistics.TotalFiles--;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveFolderAsync()
        {
            var statistics = await this.dbContext.Statistics.SingleOrDefaultAsync();

            statistics.TotalFolders--;

            await this.dbContext.SaveChangesAsync();
        }
    }
}
