namespace TaskTronic.Drive.Controllers
{
    using MassTransit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Employees;
    using Services.Employees;
    using Services.Messages;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;
    using TaskTronic.Data.Models;
    using TaskTronic.Infrastructure;
    using TaskTronic.Messages.Drive.Employees;
    using TaskTronic.Services;
    using TaskTronic.Services.Identity;

    [Authorize]
    public class EmployeesController : ApiController
    {
        private readonly ICurrentUserService currentUser;
        private readonly IEmployeeService employeeService;
        private readonly IMessageService messageService;
        private readonly IBus publisher;

        public EmployeesController(
            ICurrentUserService currentUser,
            IEmployeeService employeeService,
            IMessageService messageService,
            IBus publisher)
        {
            this.currentUser = currentUser;
            this.employeeService = employeeService;
            this.messageService = messageService;
            this.publisher = publisher;
        }

        [HttpGet]
        [Route(nameof(GetCompanyDepartmentSignId))]
        public async Task<ActionResult<int>> GetCompanyDepartmentSignId()
        {
            if (this.currentUser.IsAdministrator)
            {
                return BadRequest("Must be an employee.");
            }

            return Ok(await this.employeeService.GetCompanyDepartmentsIdAsync(this.currentUser.UserId));
        }

        [HttpPost]
        [Route(nameof(SetCompanyDepartmentSignId))]
        public async Task<ActionResult> SetCompanyDepartmentSignId(int companyId, int departmentId)
        {
            if (this.currentUser.IsAdministrator)
            {
                return BadRequest("Must be an employee.");
            }

            await this.employeeService.SetCompanyDepartmentsIdAsync(
                this.currentUser.UserId,
                companyId,
                departmentId);

            return Ok();
        }

        [HttpPut]
        [Route(Id)]
        public async Task<ActionResult> Edit(int id, InputEditEmployeeModel input)
        {
            var employee = this.currentUser.IsAdministrator
                ? await this.employeeService.FindByIdAsync(id)
                : await this.employeeService.FindByUserAsync(this.currentUser.UserId);

            if (id != employee.EmployeeId)
            {
                return BadRequest(Result.Failure(this.currentUser.UserId));
            }

            if (input is null)
            {
                return BadRequest("Input is null");
            }

            if (string.IsNullOrEmpty(input.Name) || string.IsNullOrEmpty(input.Email))
            {
                return BadRequest("Name or email is not valid.");
            }

            input.Name = input.Name.Trim();
            input.Email = input.Email.Trim();

            if (input.Name.Contains(" "))
            {
                input.Name = input.Name.Replace(" ", "_");
            }

            if (input.Email.Contains(" "))
            {
                input.Email = input.Email.Replace(" ", "_");
            }

            employee.Name = input.Name;
            employee.Email = input.Email;

            var messageData = new UserEditedMessage
            {
                UserId = employee.UserId,
                Email = input.Email,
                Name = input.Name
            };

            var message = new Message(messageData);

            await this.employeeService.SaveWithMessagesAsync(employee, message);

            await this.publisher.Publish(messageData);

            await this.messageService.MarkMessageAsPublishedAsync(message.Id);

            return Ok();
        }

        [HttpGet]
        [Route(Id)]
        public async Task<ActionResult<OutputEmployeeDetailsServiceModel>> Details(int id)
            => await this.employeeService.GetDetails(id);

        [HttpGet]
        [AuthorizeAdministrator]
        public async Task<IReadOnlyCollection<OutputEmployeeDetailsServiceModel>> All()
            => await this.employeeService.GetAllAsync();
    }
}
