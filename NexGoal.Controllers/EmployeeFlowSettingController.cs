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

    public class EmployeeFlowSettingController : Controller
    {
        private readonly ICommonService commonService;
        private readonly IEmployeeFlowSettingService flowSettingService;

        public EmployeeFlowSettingController(
            ICommonService commonService,
            IEmployeeFlowSettingService flowSettingService)
        {
            this.commonService = commonService;
            this.flowSettingService = flowSettingService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeHaveSetting(
            int flowTemplateId,
            int? groupId = null)
        {
            IEnumerable<UserModel> employees = await flowSettingService.GetEmployeeHaveSettingAsync(flowTemplateId, groupId).ConfigureAwait(false);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeDoNotHaveSetting(
            int flowTemplateId,
            int? groupId = null)
        {
            IEnumerable<UserModel> employees = await flowSettingService.GetEmployeeDoNotHaveSettingAsync(flowTemplateId, groupId).ConfigureAwait(false);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int employeeId, int flowTemplateId)
        {
            EmployeeFlowSettingModel groupFlowSetting = await flowSettingService.GetByIdAsync(employeeId, flowTemplateId).ConfigureAwait(false);

            return Json(groupFlowSetting);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int employeeId, int flowTemplateId, IEnumerable<FlowSettingModel> settings)
        {
            CommandResult commandResult = await flowSettingService.EditAsync(employeeId, flowTemplateId, settings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int employeeId, int flowTemplateId)
        {
            CommandResult commandResult = await flowSettingService.DeleteAsync(employeeId, flowTemplateId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            UserModel user = await commonService.GetEmployeeByIdAsync(employeeId).ConfigureAwait(false);

            return Json(user);
        }
    }
}