namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.ViewModels.EOQY;

    public class NominationController : Controller
    {
        private readonly ISessionService sessionService;

        public enum NavTabs
        {
            EOQ = 1,
            EOY
        };

        public NominationController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public IActionResult Index(NavTabs tab = NavTabs.EOQ)
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationManagement, Policy.Host);
            var viewModel = new NominationManagementViewModel
            {
                IsHost = isHost,
                ActiveTab = (int)tab
            };

            return View(viewModel);
        }
    }
}