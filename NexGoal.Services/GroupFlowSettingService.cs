namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.FlowSetting;
    using NexGoal.Models.FlowSetting;
    using NexGoal.Models.UserManagement;

    public class GroupFlowSettingService : IGroupFlowSettingService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public GroupFlowSettingService(
            ISessionService sessionService,
            IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        public async Task<GroupFlowSettingModel> GetByIdAsync(int groupId, int flowTempalteId)
        {
            var criteria = new GetGroupFlowSettingByIdCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTempalteId
            };

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria).ConfigureAwait(false);

            if (flowSettings == null || !flowSettings.Any())
            {
                return null;
            }

            var groupFlowSetting = new GroupFlowSettingModel
            {
                GroupId = groupId
            };

            foreach (FlowSettingModel flowSetting in flowSettings)
            {
                switch (flowSetting.ProcessId)
                {
                    case (int)PAProcess.HumanResource:
                        groupFlowSetting.HumanResources.Add(flowSetting);
                        break;

                    case (int)PAProcess.Evaluator:
                        groupFlowSetting.Evaluators.Add(flowSetting);
                        break;

                    case (int)PAProcess.Reviewer:
                        groupFlowSetting.Reviewers.Add(flowSetting);
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return groupFlowSetting;
        }

        public async Task<CommandResult> EditAsync(int groupId, int flowTemplateId, IEnumerable<FlowSettingModel> settings)
        {
            var criteria = new EditGroupFlowSettingCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTemplateId,
                FlowDetails = settings.ToXML().ToString(),
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteAsync(int groupId, int flowTemplateId)
        {
            var criteria = new DeleteGroupFlowSettingCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTemplateId,
                ModifiedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<UserModel> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new SearchUserCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataEntityAsync<UserModel>(criteria).ConfigureAwait(false);
        }
    }
}