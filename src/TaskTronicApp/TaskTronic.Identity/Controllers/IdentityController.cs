namespace TaskTronic.Identity.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;
    using Services.Identity;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;

    public class IdentityController : ApiController
    {
        private readonly ILogger<IdentityController> logger;
        private readonly IIdentityService identityService;

        public IdentityController(
            ILogger<IdentityController> logger,
            IIdentityService identityService)
        {
            this.logger = logger;
            this.identityService = identityService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register([FromBody] InputRegisterModel model)
        {
            var result = await this.identityService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                this.logger.LogInformation("Invalid registration.");
                return BadRequest(result.Errors);
            }

            this.logger.LogInformation($"Registered new user: {model.Username}");
            return this.Ok();
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<OutputJwtModel>> Login([FromBody] InputLoginModel model)
        {
            var result = await this.identityService.LoginAsync(model);

            if (!result.Succeeded)
            {
                this.logger.LogInformation("Invalid log in.");
                return BadRequest(result.Errors);
            }

            this.logger.LogInformation($"Logged in user: {model.UserName}");
            return this.Ok(result.Data);
        }
    }
}
