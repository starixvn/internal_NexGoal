namespace NexGoal.Website.Views.Shared.Components.Menu
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NexGoal.ViewModels.Auth;
    using NexGoal.Web.Core.Services;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ILogger logger;
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;

        public MenuViewComponent(
            ILogger logger,
            ISessionService sessionService,
            IAuthService authService)
        {
            this.logger = logger;
            this.sessionService = sessionService;
            this.authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            (var userInfo, var isHasMember, var unreadNotification) = await authService.GetEmployeeInfoAsync(sessionService.EmployeeId);
            var viewModel = new HeaderViewModel
            {
                MenuItems = sessionService.MenuItems,
                UserInfo = userInfo,
                IsHasMember = isHasMember,
                UnreadNotification = unreadNotification
            };

            if (sessionService.MenuItems == null)
            {
                await logger.InfoAsync($"null menu {userInfo.FullName} ({userInfo.NickName})");
            }

            return View("Menu", viewModel);
        }
    }
}