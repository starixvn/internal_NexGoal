namespace NexGoal.Website.Views.Shared.Components.Menu
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Services;
    using NexGoal.ViewModels.Auth;
    using NexGoal.Web.Core.Services;

    public class MenuViewComponent(
        ILogger logger,
        IConfiguration configuration,
        ISessionService sessionService,
        IAuthService authService,
        AppSettings appSettings) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var (userInfo, hasMember, unreadNotification) = await authService.GetEmployeeInfoAsync(sessionService.EmployeeId);
            if (userInfo == null)
            {
                userInfo = new Employee();
                await logger.InfoAsync(sessionService.EmployeeId.ToString());
            }

            var viewModel = new HeaderViewModel
            {
                UserInfo = userInfo,
                HasMember = hasMember,
                UnreadNotification = unreadNotification,
                ImageUrl = appSettings.ImageUrl,
                AppName = configuration.GetValue<string>("AppName")
            };

            return View("Menu", viewModel);
        }
    }
}