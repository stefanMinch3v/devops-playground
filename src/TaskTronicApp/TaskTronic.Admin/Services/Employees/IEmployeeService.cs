namespace TaskTronic.Admin.Services.Employees
{
    using Models.Employees;
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmployeeService
    {
        [Get("/Employees")]
        Task<IReadOnlyCollection<OutputEmployeeDetailsServiceModel>> All();

        [Get("/Employees/{id}")]
        Task<OutputEmployeeDetailsServiceModel> Details(int id);

        [Put("/Employees/{id}")]
        Task Edit(int id, InputEmployeeServiceModel employee);
    }
}
