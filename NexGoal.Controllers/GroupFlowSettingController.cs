namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    public class GroupFlowSettingController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IGroupFlowSettingService flowSettingService;

        public GroupFlowSettingController(
            ISessionService sessionService,
            IGroupFlowSettingService flowSettingService)
        {
            this.sessionService = sessionService;
            this.flowSettingService = flowSettingService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetGroupHaveSetting()
        {
            IEnumerable<GroupBaseModel> groups = await flowSettingService.GetGroupHaveSettingAsync().ConfigureAwait(false);

            return Json(groups);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetGroupFlowSetting()
        {
            IEnumerable<GroupFlowSettingModel> flowSettings = await flowSettingService.GetGroupFlowSettingAsync().ConfigureAwait(false);

            return Json(flowSettings);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetById(int groupId)
        {
            GroupFlowSettingModel flowSetting = await flowSettingService.GetByIdAsync(groupId).ConfigureAwait(false);

            return Json(flowSetting);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(int groupId, IEnumerable<FlowSettingModel> settings)
        {
            CommandResult commandResult = await flowSettingService.EditAsync(sessionService.EmployeeId, groupId, settings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int groupId)
        {
            CommandResult commandResult = await flowSettingService.DeleteAsync(sessionService.EmployeeId, groupId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}