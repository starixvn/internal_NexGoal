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

    public class EmployeeFlowSettingService : IEmployeeFlowSettingService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly IAvatarService avatarService;

        public EmployeeFlowSettingService(
            IBaseService baseService,
            ICommonService commonService,
            IAvatarService avatarService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetEmployeeHaveSettingAsync(int? groupId = null)
        {
            var criteria = new GetEmployeeHaveSettingCriteria
            {
                GroupId = groupId
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetEmployeeUnsettingAsync(int? groupId = null)
        {
            var criteria = new GetEmployeeUnsettingCriteria
            {
                GroupId = groupId
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeFlowSettingModel>> GetEmployeeFlowSettingAsync()
        {
            var criteria = new GetEmployeeFlowSettingCriteria();
            var tuple = await baseService.FetchMultipleAsync<EmployeeFlowSettingModel, FlowSettingModel>(criteria).ConfigureAwait(false);

            foreach (EmployeeFlowSettingModel employeeFlowSetting in tuple.Item1)
            {
                IEnumerable<FlowSettingModel> flowSettings = tuple.Item2.Where(x => x.EmployeeId == employeeFlowSetting.EmployeeId);
                foreach (FlowSettingModel flowSetting in flowSettings)
                {
                    switch (flowSetting.ProcessId)
                    {
                        case (int)PerformanceProcess.Evaluator:
                            employeeFlowSetting.Evaluator = flowSetting;
                            break;

                        case (int)PerformanceProcess.Reviewer:
                            employeeFlowSetting.Reviewer = flowSetting;
                            break;

                        default:
                            // nothing
                            break;
                    }
                }
            }

            return tuple.Item1;
        }

        public async Task<EmployeeFlowSettingModel> GetByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeFlowSettingByIdCriteria
            {
                EmployeeId = employeeId
            };

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria).ConfigureAwait(false);
            EmployeeBaseModel employee = await commonService.GetEmployeeByIdAsync(employeeId).ConfigureAwait(false);

            var employeeFlowSetting = new EmployeeFlowSettingModel
            {
                EmployeeId = employee.EmployeeId,
                NickName = employee.NickName,
                FullName = employee.FullName,
                Code = employee.Code,
            };

            foreach (FlowSettingModel flowSetting in flowSettings)
            {
                flowSetting.Avatar = avatarService.GetFilePath(flowSetting.Code);
                switch (flowSetting.ProcessId)
                {
                    case (int)PerformanceProcess.Evaluator:
                        employeeFlowSetting.Evaluator = flowSetting;
                        break;

                    case (int)PerformanceProcess.Reviewer:
                        employeeFlowSetting.Reviewer = flowSetting;
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return employeeFlowSetting;
        }

        public async Task<CommandResult> EditAsync(int actorId, int employeeId, IEnumerable<FlowSettingModel> settings)
        {
            var criteria = new EditEmployeeFlowSettingCriteria
            {
                EmployeeId = employeeId,
                FlowDetails = settings.ToXML().ToString(),
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> DeleteAsync(int actorId, int employeeId)
        {
            var criteria = new DeleteEmployeeFlowSettingCriteria
            {
                EmployeeId = employeeId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}