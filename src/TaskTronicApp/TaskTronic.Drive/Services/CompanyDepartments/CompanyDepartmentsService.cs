namespace TaskTronic.Drive.Services.CompanyDepartments
{
    using AutoMapper;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.CompanyDepartments;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskTronic.Common;
    using TaskTronic.Drive.Data;
    using TaskTronic.Drive.Exceptions;

    public class CompanyDepartmentsService : ICompanyDepartmentsService
    {
        private readonly IMapper mapper;
        private readonly DriveDbContext dbContext;

        public CompanyDepartmentsService(
            DriveDbContext dbContext,
            IMapper mapper)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<bool> AddDepartmentToCompany(int companyId, int departmentId)
        {
            var existingCompanyDepartment = await this.dbContext.CompanyDepartments
                .FirstOrDefaultAsync(cd => cd.CompanyId == companyId && cd.DepartmentId == departmentId);

            if (existingCompanyDepartment != null)
            {
                return false;
            }

            var companyDepartment = new CompanyDepartments
            {
                CompanyId = companyId,
                DepartmentId = departmentId
            };

            this.dbContext.CompanyDepartments
                .Add(companyDepartment);

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task CreateCompany(string name)
        {
            Guard.AgainstEmptyString<CompanyDepartmentException>(name);

            var model = new Company { Name = name };

            this.dbContext.Companies.Add(model);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task CreateDepartment(string name)
        {
            Guard.AgainstEmptyString<CompanyDepartmentException>(name);

            var model = new Department { Name = name };

            this.dbContext.Departments.Add(model);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<OutputCompaniesServiceModel> GetAllAsync(string userId)
        {
            var outputCompanies = new OutputCompaniesServiceModel
            {
                Companies = await this.mapper
                    .ProjectTo<OutputCompanyDepartmentsServiceModel>(this.dbContext.Companies)
                    .ToListAsync()
            };

            var employee = await this.dbContext.Employees
                .Include(e => e.CompanyDepartments)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee != null && employee.CompanyDepartments != null)
            {
                var selectedCompanyId = employee.CompanyDepartments.CompanyId;
                var selectedDepartmentId = employee.CompanyDepartments.DepartmentId;

                if (selectedCompanyId != 0)
                {
                    outputCompanies.SelectedData = new OutputSelectedCompanyServiceModel
                    {
                        CompanyId = selectedCompanyId,
                        DepartmentId = selectedDepartmentId
                    };
                }
            }

            return outputCompanies;
        }

        public async Task<OutputCompaniesAndDepartmentsServiceModel> GetAllAsync()
            => new OutputCompaniesAndDepartmentsServiceModel
            {
                Companies = await this.mapper
                    .ProjectTo<OutputCompanyDepartmentsServiceModel>(this.dbContext.Companies)
                    .ToListAsync(),
                Departments = await this.mapper
                    .ProjectTo<OutputDepartmentServiceModel>(this.dbContext.Departments)
                    .ToListAsync()
            };

        public async Task<IReadOnlyList<OutputDepartmentServiceModel>> AllNotInCompany(int companyId)
            => await this.mapper
                .ProjectTo<OutputDepartmentServiceModel>(this.dbContext.Departments
                    .Where(d => !d.Companies.Any(c => c.CompanyId == companyId)))
                .ToListAsync();
    }
}
