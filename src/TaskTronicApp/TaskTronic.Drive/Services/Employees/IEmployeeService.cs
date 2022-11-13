namespace TaskTronic.Drive.Services.Employees
{
    using Data.Models;
    using Models.Employees;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;
    using TaskTronic.Services;

    public interface IEmployeeService
    {
        Task<int> GetIdByUserAsync(string userId);

        Task<string> GetEmailByIdAsync(int employeeId);

        Task SaveAsync(string userId, string email, string name);

        Task<IReadOnlyCollection<OutputEmployeeDetailsServiceModel>> GetAllAsync();

        Task<OutputEmployeeDetailsServiceModel> GetDetails(int employeeId);

        Task<Employee> FindByIdAsync(int employeeId);

        Task<Employee> FindByUserAsync(string userId);

        Task<string> GetUserIdByEmployeeAsync(int employeeId);

        Task<int> GetCompanyDepartmentsIdAsync(string userId);

        Task SetCompanyDepartmentsIdAsync(string userId, int companyId, int departmentId);

        Task<int> GetSelectedCompanyDepartmentId(int employeeId);

        Task SaveWithMessagesAsync(Employee employee, params Message[] messages);
    }
}
