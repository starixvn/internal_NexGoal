namespace NexGoal.Website.Views.Shared.Components.Menu
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Auth;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ISessionService sessionService;
        private readonly ICommonService commonService;

        public MenuViewComponent(
            ISessionService sessionService,
            ICommonService commonService)
        {
            this.sessionService = sessionService;
            this.commonService = commonService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<MenuItemSession> menuItems = sessionService.MenuItems;
            ViewData["employee"] = await commonService.GetEmployeeByIdAsync(sessionService.EmployeeId);

            return View("Menu", menuItems);
        }
    }
}