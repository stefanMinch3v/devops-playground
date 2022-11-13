namespace TaskTronic.Drive.Data.Configurations
{
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder
                .HasKey(c => c.CompanyId);

            builder
                .HasMany(c => c.Departments)
                .WithOne(d => d.Company)
                .HasForeignKey(d => d.CompanyId);

            builder
                .Property(c => c.Name)
                .IsRequired();
        }
    }
}
