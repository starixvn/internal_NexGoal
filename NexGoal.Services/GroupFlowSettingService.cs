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

    public class GroupFlowSettingService : IGroupFlowSettingService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;

        public GroupFlowSettingService(
            ISessionService sessionService,
            IBaseService baseService,
            IAvatarService avatarService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<UserGroupModel>> GetGroupHaveSettingAsync(int flowTempalteId)
        {
            var criteria = new GetGroupHaveSettingCriteria
            {
                FlowTemplateId = flowTempalteId
            };

            return await baseService.GetDataAsync<UserGroupModel>(criteria).ConfigureAwait(false);
        }

        public async Task<GroupFlowSettingModel> GetByIdAsync(int groupId, int flowTempalteId)
        {
            var criteria = new GetGroupFlowSettingByIdCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTempalteId
            };

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria).ConfigureAwait(false);

            var groupFlowSetting = new GroupFlowSettingModel
            {
                GroupId = groupId
            };

            foreach (FlowSettingModel flowSetting in flowSettings)
            {
                flowSetting.Base64Image = avatarService.GetBase64Image(flowSetting.Code);
                switch (flowSetting.ProcessId)
                {
                    case (int)PerformanceProcess.HumanResource:
                        groupFlowSetting.HumanResources.Add(flowSetting);
                        break;

                    case (int)PerformanceProcess.Evaluator:
                        groupFlowSetting.Evaluator = flowSetting;
                        break;

                    case (int)PerformanceProcess.Reviewer:
                        groupFlowSetting.Reviewer = flowSetting;
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
    }
}