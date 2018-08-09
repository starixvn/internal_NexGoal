namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;

    public class GroupFlowSettingController : Controller
    {
        private readonly IGroupFlowSettingService flowSettingService;

        public GroupFlowSettingController(IGroupFlowSettingService flowSettingService)
        {
            this.flowSettingService = flowSettingService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetGroupHaveSetting(int flowTemplateId)
        {
            IEnumerable<UserGroupModel> groups = await flowSettingService.GetGroupHaveSettingAsync(flowTemplateId).ConfigureAwait(false);

            return Json(groups);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int groupId, int flowTemplateId)
        {
            GroupFlowSettingModel groupFlowSetting = await flowSettingService.GetByIdAsync(groupId, flowTemplateId).ConfigureAwait(false);

            return Json(groupFlowSetting);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int groupId, int flowTemplateId, IEnumerable<FlowSettingModel> settings)
        {
            CommandResult commandResult = await flowSettingService.EditAsync(groupId, flowTemplateId, settings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int groupId, int flowTemplateId)
        {
            CommandResult commandResult = await flowSettingService.DeleteAsync(groupId, flowTemplateId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}