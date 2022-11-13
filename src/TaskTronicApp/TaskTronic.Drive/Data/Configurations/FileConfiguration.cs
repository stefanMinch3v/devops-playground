namespace TaskTronic.Drive.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder
                .HasKey(f => f.FileId);

            builder
                .Property(f => f.BlobId)
                .IsRequired();

            builder
                .Property(f => f.CatalogId)
                .IsRequired();

            builder
                .Property(f => f.ContentType)
                .IsRequired();

            builder
                .Property(f => f.CreateDate)
                .IsRequired();

            builder
                .Property(f => f.FileName)
                .IsRequired();

            builder
                .Property(f => f.Filesize)
                .IsRequired();

            builder
                .Property(f => f.FolderId)
                .IsRequired();

            builder
                .HasOne(f => f.Employee)
                .WithMany();
        }
    }
}
