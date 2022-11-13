namespace TaskTronic.Admin.Controllers
{
    using Admin.Infrastructure;
    using Admin.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using TaskTronic.Infrastructure;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (this.User.IsAdministrator())
            {
                return RedirectToAction(nameof(EmployeesController.Index), nameof(EmployeesController).ToControllerName());
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
