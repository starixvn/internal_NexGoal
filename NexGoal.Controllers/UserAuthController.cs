namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Auth;
    using NexGoal.Services;
    using NexGoal.ViewModels.Auth;
    using NexGoal.Web.Core.Services;
    using Novell.Directory.Ldap;

    public class UserAuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;
        private readonly LdapSettings ldapSetting;

        public UserAuthController(
            ISessionService sessionService,
            IAuthService authService,
            LdapSettings ldapSetting)
        {
            this.sessionService = sessionService;
            this.authService = authService;
            this.ldapSetting = ldapSetting;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                bool isValidUser = IsValidAccountLDAP(loginModel.Username, loginModel.Password);
                if (!isValidUser)
                {
                    ModelState.AddModelError(nameof(loginModel.errorSummary), "Username or password incorrect!");
                }
                else
                {
                    var domainUsername = ldapSetting.Domain + loginModel.Username;
                    EmployeeSession employee = await authService.GetEmployeeByDomainUsernameAsync(domainUsername);

                    if(employee == null)
                    {
                        ModelState.AddModelError(nameof(loginModel.errorSummary), "User not found");
                    }
                    else
                    {
                        await DoLoginAsync(employee, loginModel);
                        return Url.IsLocalUrl(loginModel.ReturnUrl) 
                            ? Redirect(loginModel.ReturnUrl) 
                            : (ActionResult)RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(loginModel);
        }

        public async Task<IActionResult> Logout()
        {
            sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private async Task DoLoginAsync(EmployeeSession employee, LoginModel loginModel)
        {
            sessionService.EmployeeId = employee.EmployeeId;
            sessionService.NickName = employee.NickName;
            sessionService.FullName = employee.FullName;
            sessionService.IsInProbation = employee.IsInProbation;
            sessionService.Functions = await authService.GetFunctionByEmployeeIdAsync(employee.EmployeeId);
            sessionService.MenuItems = await authService.GetMenuByEmployeeIdAsync(employee.EmployeeId);

            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, loginModel.Username),
                            new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString())
                        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = loginModel.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), authProperties);
        }

        private bool IsValidAccountLDAP(string username, string password)
        {
            string userDn = $"{username}@{"luminet"}";
            using (var connection = new LdapConnection())
            {
                try
                {
                    connection.Connect(ldapSetting.LDAPUrl, LdapConnection.DefaultPort);
                    connection.Bind(userDn, password);
                    if (connection.Bound)
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
