namespace TaskTronic.Statistics.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Reflection;

    public class StatisticsDbContext : DbContext
    {
        public StatisticsDbContext(DbContextOptions<StatisticsDbContext> options)
            : base(options)
        {
        }

        public DbSet<FolderView> FolderViews { get; set; }

        public DbSet<Statistics> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
