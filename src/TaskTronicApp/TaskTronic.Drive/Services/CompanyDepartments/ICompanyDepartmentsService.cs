namespace TaskTronic.Drive.Services.CompanyDepartments
{
    using Models.CompanyDepartments;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICompanyDepartmentsService
    {
        Task<OutputCompaniesServiceModel> GetAllAsync(string userId);

        Task<OutputCompaniesAndDepartmentsServiceModel> GetAllAsync();

        Task CreateCompany(string name);

        Task CreateDepartment(string name);

        Task<IReadOnlyList<OutputDepartmentServiceModel>> AllNotInCompany(int companyId);

        Task<bool> AddDepartmentToCompany(int companyId, int departmentId);
    }
}
