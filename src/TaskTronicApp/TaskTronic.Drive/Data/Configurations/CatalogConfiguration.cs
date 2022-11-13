namespace TaskTronic.Drive.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder
                .HasKey(c => c.CatalogId);

            builder
                .HasOne(c => c.CompanyDepartments)
                .WithMany()
                .HasForeignKey(c => c.CompanyDepartmentsId);
        }
    }
}
