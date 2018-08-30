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
    using NexGoal.Models.FlowSetting;
    using NexGoal.Models.UserManagement;

    public class EmployeeFlowSettingService : IEmployeeFlowSettingService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly IAvatarService avatarService;

        public EmployeeFlowSettingService(
            ISessionService sessionService,
            IBaseService baseService,
            ICommonService commonService,
            IAvatarService avatarService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.commonService = commonService;
            this.avatarService = avatarService;
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

        public async Task<IEnumerable<UserModel>> GetEmployeeUnsettingAsync(int flowTempalteId, int? groupId = null)
        {
            var criteria = new GetEmployeeUnsettingCriteria
            {
                GroupId = groupId,
                FlowTemplateId = flowTempalteId
            };

            return await baseService.GetDataAsync<UserModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeFlowSettingModel>> GetEmployeeFlowSettingAsync(int flowTempalteId)
        {
            var criteria = new GetEmployeeFlowSettingCriteria
            {
                FlowTemplateId = flowTempalteId
            };
            var tuple = await baseService.FetchMultipleAsync<EmployeeFlowSettingModel, FlowSettingModel>(criteria).ConfigureAwait(false);

            foreach (EmployeeFlowSettingModel employeeFlowSetting in tuple.Item1)
            {
                IEnumerable<FlowSettingModel> flowSettings = tuple.Item2.Where(x => x.EmployeeId == employeeFlowSetting.EmployeeId);
                foreach (FlowSettingModel flowSetting in flowSettings)
                {
                    switch (flowSetting.ProcessId)
                    {
                        case (int)PerformanceProcess.HumanResource:
                            employeeFlowSetting.HumanResources.Add(flowSetting);
                            break;

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

        public async Task<EmployeeFlowSettingModel> GetByIdAsync(int employeeId, int flowTempalteId)
        {
            var criteria = new GetEmployeeFlowSettingByIdCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTempalteId
            };

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria).ConfigureAwait(false);
            UserModel employee = await commonService.GetEmployeeByIdAsync(employeeId).ConfigureAwait(false);

            var employeeFlowSetting = new EmployeeFlowSettingModel
            {
                EmployeeId = employee.EmployeeId,
                NickName = employee.NickName,
                FullName = employee.FullName,
                Code = employee.Code,
                Base64Image = employee.Base64Image
            };

            foreach (FlowSettingModel flowSetting in flowSettings)
            {
                flowSetting.Base64Image = avatarService.GetBase64Image(flowSetting.Code);
                switch (flowSetting.ProcessId)
                {
                    case (int)PerformanceProcess.HumanResource:
                        employeeFlowSetting.HumanResources.Add(flowSetting);
                        break;

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