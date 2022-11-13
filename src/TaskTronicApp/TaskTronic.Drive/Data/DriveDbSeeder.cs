using System.Linq;

namespace TaskTronic.Drive.Data
{
    using Microsoft.EntityFrameworkCore.Internal;
    using Models;
    using System.Collections.Generic;
    using TaskTronic.Services;

    public class DriveDbSeeder : IDbSeeder
    {
        private readonly DriveDbContext dbContext;

        public DriveDbSeeder(DriveDbContext dbContext) 
            => this.dbContext = dbContext;

        public void SeedData()
        {
            if (this.dbContext.Catalogs.Any()
                || this.dbContext.Companies.Any()
                || this.dbContext.Departments.Any())
            {
                return;
            }

            // departments
            var departments = new List<Department>
            {
                new Department { Name = "Marketing & Sales" },
                new Department { Name = "Head Management" },
                new Department { Name = "R & D" }
            };

            this.dbContext.Departments.AddRange(departments);
            this.dbContext.SaveChanges();

            // companies
            var companies = new List<Company>
            {
                new Company { Name = "Metal Slug" },
                new Company { Name = "Heroes 3 - The shadow of death" },
                new Company { Name = "Counter-Strike 1.6" }
            };

            this.dbContext.Companies.AddRange(companies);
            this.dbContext.SaveChanges();

            // connections
            var companyDepartments = new List<CompanyDepartments>();

            for (int i = 0; i < companies.Count; i++)
            {
                var cd = new CompanyDepartments
                {
                    Company = companies[i],
                    Department = departments[i]
                };

                companyDepartments.Add(cd);
            }

            this.dbContext.CompanyDepartments.AddRange(companyDepartments);
            this.dbContext.SaveChanges();
        }
    }
}
