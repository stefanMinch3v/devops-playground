namespace TaskTronic.Admin.Controllers
{
    using Admin.Infrastructure;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Identity;
    using Services.Identity;
    using System;
    using System.Threading.Tasks;
    using TaskTronic.Infrastructure;

    public class IdentityController : AdministrationController
    {
        private readonly IIdentityService identityService;
        private readonly IMapper mapper;

        public IdentityController(
            IIdentityService identityService,
            IMapper mapper)
        {
            this.identityService = identityService;
            this.mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginFormModel model)
            => await base.HandleAsync(
                async () =>
                {
                    var jwtModel = await this.identityService
                        .Login(this.mapper.Map<InputUserServiceModel>(model));

                    this.Response.Cookies.Append(
                        InfrastructureConstants.AuthenticationCookieName,
                        jwtModel.Token,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false,
                            MaxAge = TimeSpan.FromDays(1)
                        });
                },
                success: RedirectToAction(nameof(EmployeesController.Index), nameof(EmployeesController).ToControllerName()),
                failure: View("../Home/Index", model));

        [AuthorizeAdministrator]
        public IActionResult Logout()
        {
            this.Response.Cookies.Delete(InfrastructureConstants.AuthenticationCookieName);

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).ToControllerName());
        }
    }
}
