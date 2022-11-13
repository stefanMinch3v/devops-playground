namespace TaskTronic.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Refit;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskTronic.Infrastructure;

    [AuthorizeAdministrator]
    public abstract class AdministrationController : Controller
    {
        protected async Task<IActionResult> HandleAsync(Func<Task> action, IActionResult success, IActionResult failure)
        {
            try
            {
                await action();
                return success;
            }
            catch (ApiException ex)
            {
                this.ProcessErrors(ex);
                return failure;
            }
        }

        private void ProcessErrors(ApiException exception)
        {
            if (exception.HasContent)
            {
                JsonConvert
                    .DeserializeObject<List<string>>(exception.Content)
                    .ForEach(error => this.ModelState.AddModelError(string.Empty, error));
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Internal server error.");
            }
        }
    }
}
