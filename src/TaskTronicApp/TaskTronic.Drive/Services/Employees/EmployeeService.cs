namespace TaskTronic.Drive.Services.Employees
{
    using AutoMapper;
    using DapperRepo;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Employees;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;

    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper mapper;
        private readonly IFileDapper fileDapper;
        private readonly IFolderDapper folderDapper;
        private readonly DriveDbContext dbContext;

        public EmployeeService(
            DriveDbContext dbContext, 
            IMapper mapper,
            IFileDapper fileDapper,
            IFolderDapper folderDapper)
        {
            this.mapper = mapper;
            this.fileDapper = fileDapper;
            this.folderDapper = folderDapper;
            this.dbContext = dbContext;
        }

        public async Task<int> GetIdByUserAsync(string userId)
            => await this.FindByUserAsync(userId, employee => employee.EmployeeId);

        public async Task<string> GetEmailByIdAsync(int employeeId)
            => await this.dbContext.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

        public async Task SaveAsync(string userId, string email, string name)
        {
            var existing = await this.dbContext.Employees
                .FirstOrDefaultAsync(e => e.Email == email);

            if (!(existing is null) || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            this.dbContext.Employees.Add(new Employee { Email = email, UserId = userId, Name = name });

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<OutputEmployeeDetailsServiceModel>> GetAllAsync()
        {
            var employees = await this.mapper
                .ProjectTo<OutputEmployeeDetailsServiceModel>(this.dbContext.Employees)
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.TotalFiles = await this.fileDapper.CountFilesForEmployeeAsync(employee.Id);

                employee.TotalFolders = await this.folderDapper.CountFoldersForEmployeeAsync(employee.Id);
            }

            return employees;
        }

        public async Task<Employee> FindByIdAsync(int employeeId)
            => await this.dbContext.Employees.FindAsync(employeeId);

        public async Task<Employee> FindByUserAsync(string userId)
            => await this.FindByUserAsync(userId, employee => employee);

        public async Task<OutputEmployeeDetailsServiceModel> GetDetails(int employeeId)
            => await this.mapper
                .ProjectTo<OutputEmployeeDetailsServiceModel>(this.dbContext.Employees
                    .Where(d => d.EmployeeId == employeeId))
                .FirstOrDefaultAsync();

        public async Task<string> GetUserIdByEmployeeAsync(int employeeId)
            => await this.dbContext.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => e.UserId)
                .FirstOrDefaultAsync();

        public async Task<int> GetCompanyDepartmentsIdAsync(string userId)
            => (await this.FindByUserAsync(userId)).CompanyDepartmentsId;

        public async Task<int> GetSelectedCompanyDepartmentId(int employeeId)
            => await this.dbContext.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => e.CompanyDepartmentsId)
                .FirstOrDefaultAsync();
                

        public async Task SetCompanyDepartmentsIdAsync(string userId, int companyId, int departmentId)
        {
            var employee = await this.FindByUserAsync(userId);
            
            // TODO: only admin should be able to switch the employee company/department
            if (employee is null)
            {
                return;
            }

            var companyDepartment = await this.dbContext.CompanyDepartments
                .FirstOrDefaultAsync(cd => cd.CompanyId == companyId 
                    && cd.DepartmentId == departmentId);

            if (companyDepartment is null)
            {
                return;
            }

            employee.CompanyDepartmentsId = companyDepartment.Id;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task SaveWithMessagesAsync(Employee employee, params Message[] messages)
        {
            if (employee is null)
            {
                return;
            }

            foreach (var message in messages)
            {
                if (message != null)
                {
                    this.dbContext.Messages.Add(message);
                }
            }

            this.dbContext.Employees.Add(employee);

            await this.dbContext.SaveChangesAsync();
        }

        private async Task<T> FindByUserAsync<T>(
            string userId,
            Expression<Func<Employee, T>> selectorExpression)
        {
            var employeeInfo = await this.dbContext.Employees
                .Where(u => u.UserId == userId)
                .Select(selectorExpression)
                .FirstOrDefaultAsync();

            if (employeeInfo is null)
            {
                throw new InvalidOperationException("This user is not a valid employee.");
            }

            return employeeInfo;
        }
    }
}
