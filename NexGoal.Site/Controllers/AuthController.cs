namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Authentication;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;

        public AuthController()
            : this(null, null)
        {
        }

        public AuthController(ISessionService sessionService, IEmployeeService employeeService)
        {
            this.sessionService = sessionService ?? new SessionService();
            this.employeeService = employeeService ?? new EmployeeService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            HttpContext.Response.Cookies.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(string username)
        {
            var domainName = "spvn\\" + username;
            if (!string.IsNullOrEmpty(username))
            {
                var criteria = new GetEmployeeByDomainUsernameCriteria()
                {
                    DomainUsername = domainName
                };
                UserSession user = employeeService.GetEmployeeByUsername(criteria);
                if (user != null)
                {
                    this.sessionService.CurrentUser = user;
                    FormsAuthentication.SetAuthCookie(username, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User name doesn't exist!!!!!!!!!!!!!!");
                }
            }

            return View("Login", (object)username);
        }
    }
}