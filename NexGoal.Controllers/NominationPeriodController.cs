namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Services;
    using NexGoal.ViewModels.EOQY;

    public class NominationPeriodController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly INominationPeriodService nominationPeriodService;

        public enum NavTabs
        {
            EOQ = 1,
            EOY
        };

        public NominationPeriodController(
            ISessionService sessionService,
            INominationPeriodService nominationPeriodService)
        {
            this.sessionService = sessionService;
            this.nominationPeriodService = nominationPeriodService;
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

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int id)
        {
            CommandResult commandResult = await nominationPeriodService.DeleteAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}