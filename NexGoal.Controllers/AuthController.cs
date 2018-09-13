namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Criteria;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public AuthController(
            ISessionService sessionService,
            IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        // [NonAction]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // [NonAction]
        [HttpPost]
        public ActionResult Login(string username)
        {
            var domainUsername = "spvn\\" + username;
            if (!string.IsNullOrEmpty(username))
            {
                EmployeeSession employee = GetEmployeeByDomainUsername(domainUsername);

                if (employee != null)
                {
                    sessionService.EmployeeId = employee.EmployeeId;
                    sessionService.NickName = employee.NickName;
                    sessionService.FullName = employee.FullName;
                    sessionService.Functions = GetFunctionByEmployeeId(employee.EmployeeId);
                    sessionService.MenuItems = GetMenuByEmployeeId(employee.EmployeeId);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View("Login", username);
        }

        private EmployeeSession GetEmployeeByDomainUsername(string domainUsername)
        {
            var criteria = new GetEmployeeByDomainUsernameCriteria
            {
                DomainUsername = domainUsername
            };

            return baseService.GetDataEntityAsync<EmployeeSession>(criteria).Result;
        }

        private IEnumerable<FunctionSession> GetFunctionByEmployeeId(int employeeId)
        {
            var criteria = new GetFunctionByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return baseService.GetDataAsync<FunctionSession>(criteria).Result;
        }

        private IEnumerable<MenuItemSession> GetMenuByEmployeeId(int employeeId)
        {
            var criteria = new GetMenuByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            IEnumerable<MenuItemSession> menuItems = baseService.GetDataAsync<MenuItemSession>(criteria).Result;
            IEnumerable<MenuItemSession> menuGroups = menuItems.Where(x => x.ParentId == null);
            foreach (MenuItemSession menuGroup in menuGroups)
            {
                menuGroup.Childs = menuItems.Where(x => x.ParentId == menuGroup.MenuId);
            }

            return menuGroups;
        }
    }
}