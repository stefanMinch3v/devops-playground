namespace TaskTronic.Drive.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.CompanyDepartments;
    using Services.CompanyDepartments;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;
    using TaskTronic.Infrastructure;
    using TaskTronic.Services.Identity;

    public class CompaniesController : ApiController
    {
        private readonly ICompanyDepartmentsService companyDepartments;
        private readonly ICurrentUserService currentUser;

        public CompaniesController(
            ICompanyDepartmentsService companyDepartments,
            ICurrentUserService currentUser)
        {
            this.companyDepartments = companyDepartments;
            this.currentUser = currentUser;
        }

        [HttpGet]
        [Route(nameof(GetCompanyDepartments))]
        public async Task<ActionResult<OutputCompaniesServiceModel>> GetCompanyDepartments()
            => await this.companyDepartments.GetAllAsync(this.currentUser.UserId);

        [HttpPost]
        [Route(nameof(Create))]
        [AuthorizeAdministrator]
        public async Task<ActionResult> Create(string name)
        {
            await this.companyDepartments.CreateCompany(name);
            return Ok();
        }

        [HttpPost]
        [Route(nameof(CreateDepartment))]
        [AuthorizeAdministrator]
        public async Task<ActionResult> CreateDepartment(string name)
        {
            await this.companyDepartments.CreateDepartment(name);
            return Ok();
        }

        [HttpGet]
        [Route(nameof(All))]
        [AuthorizeAdministrator]
        public async Task<OutputCompaniesAndDepartmentsServiceModel> All()
            => await this.companyDepartments.GetAllAsync();

        [HttpGet]
        [Route(nameof(AllNotInCompany))]
        [AuthorizeAdministrator]
        public async Task<IReadOnlyList<OutputDepartmentServiceModel>> AllNotInCompany(int companyId)
            => await this.companyDepartments.AllNotInCompany(companyId);

        [HttpPost]
        [Route(nameof(AddDepartmentToCompany))]
        [AuthorizeAdministrator]
        public async Task<ActionResult<bool>> AddDepartmentToCompany(int companyId, int departmentId)
            => Ok(await this.companyDepartments.AddDepartmentToCompany(companyId, departmentId));
    }
}
