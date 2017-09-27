namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IAuthenticationService authenticationService;

        public AuthController()
            : this(null, null)
        {
        }

        public AuthController(ISessionService sessionService, IAuthenticationService authenticationService)
        {
            this.sessionService = sessionService ?? new SessionService();
            this.authenticationService = authenticationService ?? new AuthenticationService();
        }

#if(RELEASE)
        [NonAction]
#endif
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

#if (RELEASE)
        [NonAction]
#endif
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            HttpContext.Response.Cookies.Clear();

            return RedirectToAction("Login");
        }

#if (RELEASE)
        [NonAction]
#endif
        [HttpPost]
        public ActionResult Login(string username)
        {
            var domainName = "spvn\\" + username;
            if (!string.IsNullOrEmpty(username))
            {
                UserSession user = authenticationService.GetEmployeeByUsername(domainName);
                if (user != null)
                {
                    this.sessionService.CurrentUser = user;
                    FormsAuthentication.SetAuthCookie(username, false);
                    authenticationService.InsertLogLogin(username);

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