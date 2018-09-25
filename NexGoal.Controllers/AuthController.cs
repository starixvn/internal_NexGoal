namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IAuthenticationService authenticationService;

        public AuthController(
            ISessionService sessionService,
            IAuthenticationService authenticationService)
        {
            this.sessionService = sessionService;
            this.authenticationService = authenticationService;
        }

        // [NonAction]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // [NonAction]
        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            var domainUsername = "spvn\\" + username;
            if (!string.IsNullOrEmpty(username))
            {
                EmployeeSession employee = await authenticationService.GetEmployeeByDomainUsernameAsync(domainUsername).ConfigureAwait(false);

                if (employee != null)
                {
                    sessionService.EmployeeId = employee.EmployeeId;
                    sessionService.NickName = employee.NickName;
                    sessionService.FullName = employee.FullName;
                    sessionService.Functions = await authenticationService.GetFunctionByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);
                    sessionService.MenuItems = await authenticationService.GetMenuByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View("Login", username);
        }
    }
}