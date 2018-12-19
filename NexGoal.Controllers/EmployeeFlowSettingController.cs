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
        public async Task<JsonResult> GetEmployeeHaveSetting(int? groupId = null)
        {
            IEnumerable<EmployeeBaseModel> employees = await flowSettingService.GetEmployeeHaveSettingAsync(groupId).ConfigureAwait(false);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeUnsetting(int? groupId = null)
        {
            IEnumerable<EmployeeBaseModel> employees = await flowSettingService.GetEmployeeUnsettingAsync(groupId).ConfigureAwait(false);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeFlowSetting()
        {
            IEnumerable<EmployeeFlowSettingModel> flowSettings = await flowSettingService.GetEmployeeFlowSettingAsync().ConfigureAwait(false);

            return Json(flowSettings);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetById(int employeeId)
        {
            EmployeeFlowSettingModel flowSetting = await flowSettingService.GetByIdAsync(employeeId).ConfigureAwait(false);

            return Json(flowSetting);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(int employeeId, IEnumerable<FlowSettingModel> settings)
        {
            CommandResult commandResult = await flowSettingService.EditAsync(employeeId, settings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int employeeId)
        {
            CommandResult commandResult = await flowSettingService.DeleteAsync(employeeId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}