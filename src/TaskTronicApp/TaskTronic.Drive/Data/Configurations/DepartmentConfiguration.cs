namespace TaskTronic.Drive.Data.Configurations
{
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder
                .HasKey(d => d.DepartmentId);

            builder
                .HasMany(d => d.Companies)
                .WithOne(c => c.Department)
                .HasForeignKey(c => c.DepartmentId);

            builder
                .Property(d => d.Name)
                .IsRequired();
        }
    }
}
