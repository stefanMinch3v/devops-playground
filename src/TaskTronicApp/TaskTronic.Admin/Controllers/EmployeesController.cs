namespace TaskTronic.Admin.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Models.Employees;
    using Services.Employees;
    using System.Threading.Tasks;

    public class EmployeesController : AdministrationController
    {
        private readonly IEmployeeService employeeService;
        private readonly IMapper mapper;

        public EmployeesController(
            IEmployeeService employeeService,
            IMapper mapper)
        {
            this.employeeService = employeeService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
            => View(await this.employeeService.All());

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await this.employeeService.Details(id);

            var formModel = this.mapper.Map<EmployeeFormModel>(employee);

            return View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeFormModel model)
            => await this.HandleAsync(async () =>
                    await this.employeeService.Edit(id, this.mapper.Map<InputEmployeeServiceModel>(model)),
                    success: RedirectToAction(nameof(Index)),
                    failure: View(model));
    }
}
