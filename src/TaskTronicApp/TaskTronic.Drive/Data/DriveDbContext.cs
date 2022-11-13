namespace TaskTronic.Drive.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;
    using Models;
    using TaskTronic.Data;

    public class DriveDbContext : MessageDbContext
    {
        public DriveDbContext(DbContextOptions<DriveDbContext> options)
            : base(options)
        {
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Blobsdata> Blobsdata { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CompanyDepartments> CompanyDepartments { get; set; }

        protected override Assembly ConfigurationsAssembly => Assembly.GetExecutingAssembly();
    }
}
