namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;

    public class NominationManagementController : Controller
    {
        private readonly ISessionService sessionService;

        public NominationManagementController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public IActionResult Index()
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationManagement, Policy.Host);

            return View(isHost);
        }
    }
}