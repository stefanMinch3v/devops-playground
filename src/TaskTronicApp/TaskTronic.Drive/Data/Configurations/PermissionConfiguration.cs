namespace TaskTronic.Drive.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder
                .HasKey(p => p.PermissionId);

            builder
                .Property(p => p.CatalogId)
                .IsRequired();

            builder
                .Property(p => p.FolderId)
                .IsRequired();

            builder
                .HasOne(p => p.Employee)
                .WithMany();
        }
    }
}
