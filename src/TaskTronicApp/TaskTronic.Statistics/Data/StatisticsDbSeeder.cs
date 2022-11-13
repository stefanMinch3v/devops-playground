namespace TaskTronic.Statistics.Data
{
    using System.Linq;
    using Models;
    using TaskTronic.Services;

    public class StatisticsDbSeeder : IDbSeeder
    {
        private readonly StatisticsDbContext dbContext;

        public StatisticsDbSeeder(StatisticsDbContext db)
            => this.dbContext = db;

        public void SeedData()
        {
            if (this.dbContext.Statistics.Any())
            {
                return;
            }

            this.dbContext.Statistics.Add(new Statistics
            {
                TotalFolders = 0,
                TotalFiles = 0
            });

            this.dbContext.SaveChanges();
        }
    }
}
