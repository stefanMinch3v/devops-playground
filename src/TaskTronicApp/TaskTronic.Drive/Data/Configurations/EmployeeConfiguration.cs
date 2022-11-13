namespace TaskTronic.Drive.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder
                .HasKey(e => e.EmployeeId);

            builder
                .Property(e => e.Email)
                .IsRequired();

            builder
                .Property(e => e.UserId)
                .IsRequired();

            builder
                .Property(e => e.Name)
                .IsRequired();

            builder
                .HasOne(e => e.CompanyDepartments)
                .WithMany();
        }
    }
}
