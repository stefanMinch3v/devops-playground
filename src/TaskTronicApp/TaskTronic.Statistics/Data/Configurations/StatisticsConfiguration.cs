namespace TaskTronic.Statistics.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class StatisticsConfiguration : IEntityTypeConfiguration<Statistics>
    {
        public void Configure(EntityTypeBuilder<Statistics> builder)
        {
            builder
                .HasKey(v => v.StatisticsId);

            builder
                .Property(v => v.TotalFolders)
                .IsRequired();

            builder
                .Property(v => v.TotalFiles)
                .IsRequired();
        }
    }
}
