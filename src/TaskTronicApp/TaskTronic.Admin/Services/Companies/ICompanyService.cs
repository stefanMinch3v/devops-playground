namespace TaskTronic.Admin.Services.Companies
{
    using Models.Companies;
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICompanyService
    {
        [Post("/Companies/CreateDepartment")]
        Task CreateDepartment(string name);

        [Post("/Companies/Create")]
        Task Create(string name);

        [Get("/Companies/All")]
        Task<OutputCompaniesAndDepartmentsServiceModel> All();

        [Get("/Companies/AllNotInCompany")]
        Task<IReadOnlyList<OutputDepartmentServiceModel>> AllNotInCompany(int companyId);

        [Post("/Companies/AddDepartmentToCompany")]
        Task<bool> AddDepartmentToCompany(int companyId, int departmentId);
    }
}
