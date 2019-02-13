namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;
    using NexGoal.ViewModels.EOQY;
    using NexGoal.ViewModels.NominationPeriod;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Read)]
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

        public IActionResult Index(NavTabs tab = NavTabs.EOQ)
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationPeriod, Policy.Host);
            var viewModel = new NominationManagementViewModel
            {
                IsHost = isHost,
                ActiveTab = (int)tab
            };

            return View(viewModel);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int id)
        {
            CommandResult commandResult = await nominationPeriodService.DeleteAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        public async Task<JsonResult> GetRoleSetting(GetRoleSettingParam param)
        {
            IEnumerable<RoleSettingModel> nominators = await nominationPeriodService.GetRoleSettingAsync(param).ConfigureAwait(false);
            IEnumerable<RoleSettingViewModel> viewModels = nominators.Select(x => new RoleSettingViewModel
            {
                EmployeeId = x.EmployeeId,
                NickName = x.NickName,
                FullName = x.FullName,
                GroupName = x.GroupName,
                JobTitle = x.JobTitle,
                RoleIds = x.RoleIds
            });

            return Json(viewModels);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditRoleSetting(EditRoleSettingModel setting)
        {
            CommandResult commandResult = await nominationPeriodService.EditRoleSettingAsync(setting, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}