namespace TaskTronic.Drive.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class BlobsdataConfiguration : IEntityTypeConfiguration<Blobsdata>
    {
        public void Configure(EntityTypeBuilder<Blobsdata> builder)
        {
            builder
                .HasKey(f => f.BlobId);

            builder
                .Property(f => f.Data)
                .IsRequired();

            builder
                .Property(f => f.FileName)
                .IsRequired();

            builder
                .Property(f => f.FileSize)
                .IsRequired();

            builder
                .Property(f => f.FinishedUpload)
                .IsRequired();

            builder
                .Property(f => f.EmployeeId)
                .IsRequired();

            builder
               .Property(f => f.Timestamp)
               .IsRequired();
        }
    }
}
