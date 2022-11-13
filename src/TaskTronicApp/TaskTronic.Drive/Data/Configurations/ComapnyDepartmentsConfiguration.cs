namespace TaskTronic.Drive.Data.Configurations
{
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ComapnyDepartmentsConfiguration : IEntityTypeConfiguration<CompanyDepartments>
    {
        public void Configure(EntityTypeBuilder<CompanyDepartments> builder)
        {
            builder
                .HasKey(cd => cd.Id);

            builder
                .HasOne(cd => cd.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(c => c.DepartmentId);

            builder
                .HasOne(cd => cd.Department)
                .WithMany(d => d.Companies)
                .HasForeignKey(d => d.CompanyId);
        }
    }
}
