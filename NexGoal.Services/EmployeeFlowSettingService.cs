namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowSetting;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Models.UserManagement;

    public class EmployeeFlowSettingService : IEmployeeFlowSettingService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public EmployeeFlowSettingService(
            ISessionService sessionService,
            IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UserModel>> GetEmployeeHaveSettingAsync(int flowTempalteId, int? groupId = null)
        {
            var criteria = new GetEmployeeHaveSettingCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTempalteId
            };

            return await baseService.GetDataAsync<UserModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserModel>> GetEmployeeDoNotHaveSettingAsync(int flowTempalteId, int? groupId = null)
        {
            var criteria = new GetEmployeeDoNotHaveSettingCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTempalteId
            };

            return await baseService.GetDataAsync<UserModel>(criteria).ConfigureAwait(false);
        }

        public async Task<EmployeeFlowSettingModel> GetByIdAsync(int employeeId, int flowTempalteId)
        {
            var criteria = new GetEmployeeFlowSettingByIdCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTempalteId
            };

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria).ConfigureAwait(false);

            var employeeFlowSetting = new EmployeeFlowSettingModel
            {
                EmployeeId = employeeId
            };

            foreach (FlowSettingModel flowSetting in flowSettings)
            {
                switch (flowSetting.ProcessId)
                {
                    case (int)PAProcess.HumanResource:
                        employeeFlowSetting.HumanResources.Add(flowSetting);
                        break;

                    case (int)PAProcess.Evaluator:
                        employeeFlowSetting.Evaluators.Add(flowSetting);
                        break;

                    case (int)PAProcess.Reviewer:
                        employeeFlowSetting.Reviewers.Add(flowSetting);
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return employeeFlowSetting;
        }

        public async Task<CommandResult> EditAsync(int employeeId, int flowTemplateId, IEnumerable<FlowSettingModel> settings)
        {
            var criteria = new EditEmployeeFlowSettingCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId,
                FlowDetails = settings.ToXML().ToString(),
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteAsync(int employeeId, int flowTemplateId)
        {
            var criteria = new DeleteEmployeeFlowSettingCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}