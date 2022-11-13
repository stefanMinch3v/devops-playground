namespace TaskTronic.Statistics.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class FolderViewConfiguration : IEntityTypeConfiguration<FolderView>
    {
        public void Configure(EntityTypeBuilder<FolderView> builder)
        {
            builder
                .HasKey(v => v.FolderViewId);

            builder
                .HasIndex(v => v.FolderId);

            builder
                .Property(v => v.UserId)
                .IsRequired();
        }
    }
}
