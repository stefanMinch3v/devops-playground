namespace TaskTronic.Drive.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class FolderConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder
                .HasKey(f => f.FolderId);

            builder
                .Property(f => f.CatalogId)
                .IsRequired();

            builder
                .Property(f => f.IsPrivate)
                .IsRequired()
                .HasDefaultValue(false);

            builder
                .Property(f => f.CreateDate)
                .IsRequired();

            builder
                .HasOne(f => f.Employee)
                .WithMany();
        }
    }
}
