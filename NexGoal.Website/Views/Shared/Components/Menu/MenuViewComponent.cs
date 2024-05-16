namespace NexGoal.Website.Views.Shared.Components.Menu
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Models;
    using NexGoal.Services;
    using NexGoal.ViewModels.Auth;
    using NexGoal.Web.Core.Services;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ISessionService sessionService;
        private readonly IAuthService authService;
        private readonly IConfiguration configuration;
        private readonly AppSettings appSettings;

        public MenuViewComponent(
            IConfiguration configuration,
            ISessionService sessionService,
            IAuthService authService,
            AppSettings appSettings)
        {
            this.configuration = configuration;
            this.sessionService = sessionService;
            this.authService = authService;
            this.appSettings = appSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            (var userInfo, var hasMember, var unreadNotification) = await authService.GetEmployeeInfoAsync(sessionService.EmployeeId);
            var viewModel = new HeaderViewModel
            {
                MenuItems = sessionService.MenuItems,
                UserInfo = userInfo,
                HasMember = hasMember,
                UnreadNotification = unreadNotification,
                ImageUrl = appSettings.ImageUrl,
                LogoName = configuration.GetValue<string>("CompanyName").Equals(Constants.LumiCompanyName) ? "lumigoal.svg" : "nexgoal.svg"
            };

            return View("Menu", viewModel);
        }
    }
}