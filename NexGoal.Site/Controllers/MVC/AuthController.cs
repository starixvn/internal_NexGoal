namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using System.Configuration;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IAuthenticationService employeeService;

        public AuthController()
            : this(null, null)
        {
        }

        public AuthController(ISessionService sessionService, IAuthenticationService authenticationService)
        {
            this.sessionService = sessionService ?? new SessionService();
            this.employeeService = authenticationService ?? new AuthenticationService();
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
            var canLogin = ConfigurationManager.AppSettings["IsSimulatedLoginUser"].ToString();
            if (canLogin == "true")
            {
                var domainName = "spvn\\" + username;
                if (!string.IsNullOrEmpty(username))
                {
                    UserSession user = employeeService.GetEmployeeByUsername(domainName);
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
            }

            return View("Login", (object)username);
        }
    }
}