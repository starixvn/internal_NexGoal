namespace NexGoal.Controllers;

using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NexGoal.Core.Models;
using NexGoal.Models.Auth;
using NexGoal.Services;
using NexGoal.ViewModels.Auth;
using NexGoal.Web.Core.Services;
using Novell.Directory.Ldap;

public class AuthController(
    ISessionService sessionService,
    IAuthService authService,
    AppSettings appSettings,
    LdapSettings ldapSetting) : Controller
{
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
            var isValidUser = appSettings.LoginType == 1
                ? IsValidAccountLDAP(loginModel.Username, loginModel.Password)
                : await IsValidLoginByPassword(loginModel);
            if (!isValidUser)
            {
                ModelState.AddModelError(nameof(loginModel.errorSummary), "Username or password incorrect!");
            }
            else
            {
                var domainUsername = ldapSetting.Domain + loginModel.Username;
                var employee = await authService.GetEmployeeByDomainUsernameAsync(domainUsername);

                if (employee == null)
                {
                    ModelState.AddModelError(nameof(loginModel.errorSummary), "User not found");
                }
                else
                {
                    await DoLoginAsync(employee, loginModel);
                    return Url.IsLocalUrl(loginModel.ReturnUrl)
                        ? Redirect(loginModel.ReturnUrl)
                        : RedirectToAction("Index", "Home");
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
            new(ClaimTypes.Name, loginModel.Username),
            new(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = loginModel.RememberMe
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

        await WriteLogAsync(loginModel.Username);
    }

    private bool IsValidAccountLDAP(string username, string password)
    {
        bool isValid;
        using var connection = new LdapConnection();
        try
        {
            connection.Connect(ldapSetting.Host, ldapSetting.Port);
            connection.Bind(string.Format(ldapSetting.LoginSchema, username), password);
            isValid = connection.Bound;
        }
        catch
        {
            isValid = false;
        }

        return isValid;
    }

    private async Task<bool> IsValidLoginByPassword(LoginModel model)
    {
        var employee = await authService.GetEmployeeByDomainUsernameAsync(model.Username);
        return employee != null && !string.IsNullOrWhiteSpace(employee.Password) && BCrypt.Net.BCrypt.Verify(model.Password, employee.Password);
    }

    private async Task WriteLogAsync(string username)
    {
        var logInfo = new LogLogin
        {
            Username = username,
            IP = HttpContext.Connection?.RemoteIpAddress?.ToString(),
            Platform = HttpContext.Request?.Headers["User-Agent"].ToString()
        };
        logInfo.MachineName = GetMachineName(logInfo.IP);

        await authService.WriteLogLoginAsync(logInfo);
    }

    private static string GetMachineName(string ipAdress)
    {
        var machineName = string.Empty;
        try
        {
            var hostEntry = Dns.GetHostEntry(ipAdress);
            machineName = hostEntry.HostName;
        }
        catch
        {
            // log here
        }

        return machineName;
    }
}