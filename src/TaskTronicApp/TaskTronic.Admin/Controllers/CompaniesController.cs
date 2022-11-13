namespace TaskTronic.Admin.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Services.Companies;
    using System.Threading.Tasks;
    using TaskTronic.Admin.Models.Companies;

    public class CompaniesController : AdministrationController
    {
        private readonly ICompanyService companyService;
        private readonly IMapper mapper;

        public CompaniesController(
            ICompanyService employeeService,
            IMapper mapper)
        {
            this.companyService = employeeService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
            => View(await this.companyService.All());

        [HttpGet]
        public IActionResult Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateFormViewModel model)
            => await this.HandleAsync(async () =>
                    await this.companyService.Create(model.Name),
                    success: RedirectToAction(nameof(Index)),
                    failure: View(model));

        [HttpGet]
        public IActionResult CreateDepartment()
            => View();

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(CreateFormViewModel model)
            => await this.HandleAsync(async () =>
                    await this.companyService.CreateDepartment(model.Name),
                    success: RedirectToAction(nameof(Index)),
                    failure: View(model));

        [HttpGet]
        public async Task<IActionResult> AttachDepartment(int companyId)
            => View(await this.GetAllDepartmentsNotInCompanyModelAsync(companyId));

        public async Task<IActionResult> AttachDepartments(int companyId, int departmentId)
            => await this.HandleAsync(async () =>
                    await this.companyService.AddDepartmentToCompany(companyId, departmentId),
                    success: RedirectToAction(nameof(Index)),
                    failure: View(new { companyId, departmentId }));

        private async Task<CompanyDepartmentListViewModel> GetAllDepartmentsNotInCompanyModelAsync(int companyId)
            => new CompanyDepartmentListViewModel
            {
                CompanyId = companyId,
                Departments = await this.companyService.AllNotInCompany(companyId)
            };
    }
}
