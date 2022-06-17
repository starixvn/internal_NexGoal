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

    public class EmployeeFlowSettingController : BaseController
    {
        private readonly IEmployeeFlowSettingService flowSettingService;

        public EmployeeFlowSettingController(
            ISessionService sessionService,
            IEmployeeFlowSettingService flowSettingService) : base(sessionService)
        {
            this.flowSettingService = flowSettingService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeHaveSetting(int? groupId = null)
        {
            IEnumerable<Employee> employees = await flowSettingService.GetEmployeeHaveSettingAsync(groupId);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeUnsetting(int? groupId = null)
        {
            IEnumerable<Employee> employees = await flowSettingService.GetEmployeeUnsettingAsync(groupId);

            return Json(employees);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeFlowSetting()
        {
            IEnumerable<EmployeeFlowSettingModel> flowSettings = await flowSettingService.GetEmployeeFlowSettingAsync();

            return Json(flowSettings);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetById(int employeeId)
        {
            EmployeeFlowSettingModel flowSetting = await flowSettingService.GetByIdAsync(employeeId);

            return Json(flowSetting);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(int employeeId, IEnumerable<FlowSettingModel> settings)
        {
            CommandResult commandResult = await flowSettingService.EditAsync(sessionService.EmployeeId, employeeId, settings);

            return Json(commandResult);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int employeeId)
        {
            CommandResult commandResult = await flowSettingService.DeleteAsync(sessionService.EmployeeId, employeeId);

            return Json(commandResult);
        }
    }
}