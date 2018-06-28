namespace NexGoal.Website.Views.Shared.Components.Menu
{
    using System.Threading.Tasks;
    using Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ISessionService sessionService;

        public MenuViewComponent(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["NickName"] = sessionService.NickName;
            ViewData["FullName"] = sessionService.FullName;
            var menuItems = await Task.FromResult(sessionService.MenuItems);

            return View("Menu", menuItems);
        }
    }
}