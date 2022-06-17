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

        public GroupFlowSettingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Group>> GetGroupHaveSettingAsync()
        {
            var criteria = new GetGroupHaveSettingCriteria();

            return await baseService.GetDataAsync<Group>(criteria);
        }

        public async Task<IEnumerable<GroupFlowSettingModel>> GetGroupFlowSettingAsync()
        {
            var criteria = new GetGroupFlowSettingCriteria();
            var tuple = await baseService.FetchMultipleAsync<GroupFlowSettingModel, FlowSettingModel>(criteria);

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

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria);

            var groupFlowSetting = new GroupFlowSettingModel
            {
                GroupId = groupId
            };

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

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeleteAsync(int actorId, int groupId)
        {
            var criteria = new DeleteGroupFlowSettingCriteria
            {
                GroupId = groupId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<IEnumerable<GroupModel>> GetGroupsAsync()
        {
            var criteria = new GetGroupsCriteria();

            return await baseService.GetDataAsync<GroupModel>(criteria);
        }
    }
}