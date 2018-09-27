namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Services;

    public class EmployeeFlowSettingController : Controller
    {
        private readonly IEmployeeFlowSettingService flowSettingService;

        public EmployeeFlowSettingController(IEmployeeFlowSettingService flowSettingService)
        {
            this.flowSettingService = flowSettingService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeHaveSetting(
            int flowTemplateId,
            int? groupId = null)
        {
            IEnumerable<EmployeeBaseModel> employees = await flowSettingService.GetEmployeeHaveSettingAsync(flowTemplateId, groupId).ConfigureAwait(false);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeUnsetting(
            int flowTemplateId,
            int? groupId = null)
        {
            IEnumerable<EmployeeBaseModel> employees = await flowSettingService.GetEmployeeUnsettingAsync(flowTemplateId, groupId).ConfigureAwait(false);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeFlowSetting(int flowTemplateId)
        {
            IEnumerable<EmployeeFlowSettingModel> flowSettings = await flowSettingService.GetEmployeeFlowSettingAsync(flowTemplateId).ConfigureAwait(false);

            return Json(flowSettings);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetById(int employeeId, int flowTemplateId)
        {
            EmployeeFlowSettingModel flowSetting = await flowSettingService.GetByIdAsync(employeeId, flowTemplateId).ConfigureAwait(false);

            return Json(flowSetting);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(int employeeId, int flowTemplateId, IEnumerable<FlowSettingModel> settings)
        {
            CommandResult commandResult = await flowSettingService.EditAsync(employeeId, flowTemplateId, settings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int employeeId, int flowTemplateId)
        {
            CommandResult commandResult = await flowSettingService.DeleteAsync(employeeId, flowTemplateId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}