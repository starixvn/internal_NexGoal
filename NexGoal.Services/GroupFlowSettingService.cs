namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowSetting;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;

    public class GroupFlowSettingService : IGroupFlowSettingService
    {
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;

        public GroupFlowSettingService(
            IBaseService baseService,
            IAvatarService avatarService)
        {
            this.baseService = baseService;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupHaveSettingAsync()
        {
            var criteria = new GetGroupHaveSettingCriteria();

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupFlowSettingModel>> GetGroupFlowSettingAsync()
        {
            var criteria = new GetGroupFlowSettingCriteria();
            var tuple = await baseService.FetchMultipleAsync<GroupFlowSettingModel, FlowSettingModel>(criteria).ConfigureAwait(false);

            foreach (GroupFlowSettingModel groupFlowSetting in tuple.Item1)
            {
                IEnumerable<FlowSettingModel> flowSettings = tuple.Item2.Where(x => x.GroupId == groupFlowSetting.GroupId);
                foreach (FlowSettingModel flowSetting in flowSettings)
                {
                    switch (flowSetting.ProcessId)
                    {
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
            }

            return tuple.Item1;
        }

        public async Task<GroupFlowSettingModel> GetByIdAsync(int groupId)
        {
            var criteria = new GetGroupFlowSettingByIdCriteria
            {
                GroupId = groupId
            };

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria).ConfigureAwait(false);

            var groupFlowSetting = new GroupFlowSettingModel
            {
                GroupId = groupId
            };

            foreach (FlowSettingModel flowSetting in flowSettings)
            {
                flowSetting.Avatar = avatarService.GetFilePath(flowSetting.Code);
                switch (flowSetting.ProcessId)
                {
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

        public async Task<CommandResult> EditAsync(int actorId, int groupId, IEnumerable<FlowSettingModel> settings)
        {
            var criteria = new EditGroupFlowSettingCriteria
            {
                GroupId = groupId,
                FlowDetails = settings.ToXML().ToString(),
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteAsync(int actorId, int groupId)
        {
            var criteria = new DeleteGroupFlowSettingCriteria
            {
                GroupId = groupId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}