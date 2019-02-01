namespace NexGoal.Website.Views.Shared.Components.Menu
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NexGoal.ViewModels.Auth;
    using NexGoal.Web.Core.Services;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;

        public MenuViewComponent(
            ISessionService sessionService,
            IAuthService authService)
        {
            this.sessionService = sessionService;
            this.authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var employeeInfo = await authService.GetEmployeeInfo(sessionService.EmployeeId);
            var viewModel = new HeaderViewModel
            {
                MenuItems = sessionService.MenuItems,
                CurrentUserInfo = employeeInfo.Item1,
                IsHasMember = employeeInfo.Item2,
                UnreadNotification = employeeInfo.Item3
            };

            return View("Menu", viewModel);
        }
    }
}