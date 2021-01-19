namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Auth;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;

        public AuthController(
            ISessionService sessionService,
            IAuthService authService)
        {
            this.sessionService = sessionService;
            this.authService = authService;
        }

        //[NonAction]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

       //[NonAction]
        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            var domainUsername = "spvn\\" + username;
            if (!string.IsNullOrEmpty(username))
            {
                EmployeeSession employee = await authService.GetEmployeeByDomainUsernameAsync(domainUsername).ConfigureAwait(false);

                if (employee != null)
                {
                    sessionService.EmployeeId = employee.EmployeeId;
                    sessionService.NickName = employee.NickName;
                    sessionService.FullName = employee.FullName;
                    sessionService.IsInProbation = employee.IsInProbation;
                    sessionService.Functions = await authService.GetFunctionByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);
                    sessionService.MenuItems = await authService.GetMenuByEmployeeIdAsync(employee.EmployeeId).ConfigureAwait(false);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View("Login", username);
        }
    }
}