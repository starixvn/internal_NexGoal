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
    using NexGoal.Repositories.Interfaces;

    public class EmployeeFlowSettingService : IEmployeeFlowSettingService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public EmployeeFlowSettingService(
            IBaseService baseService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<Employee>> GetEmployeeHaveSettingAsync(int? groupId = null)
        {
            var criteria = new GetEmployeeHaveSettingCriteria
            {
                GroupId = groupId
            };

            return await baseService.GetDataAsync<Employee>(criteria);
        }

        public async Task<IEnumerable<Employee>> GetEmployeeUnsettingAsync(int? groupId = null)
        {
            var criteria = new GetEmployeeUnsettingCriteria
            {
                GroupId = groupId
            };

            return await baseService.GetDataAsync<Employee>(criteria);
        }

        public async Task<IEnumerable<EmployeeFlowSettingModel>> GetEmployeeFlowSettingAsync()
        {
            var criteria = new GetEmployeeFlowSettingCriteria();
            var tuple = await baseService.FetchMultipleAsync<EmployeeFlowSettingModel, FlowSettingModel>(criteria);

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

            var flowSettings = await baseService.GetDataAsync<FlowSettingModel>(criteria);
            Employee employee = await commonService.GetEmployeeByIdAsync(employeeId);

            var employeeFlowSetting = new EmployeeFlowSettingModel
            {
                EmployeeId = employee.EmployeeId,
                NickName = employee.NickName,
                FullName = employee.FullName,
                Code = employee.Code
            };

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

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeleteAsync(int actorId, int employeeId)
        {
            var criteria = new DeleteEmployeeFlowSettingCriteria
            {
                EmployeeId = employeeId,
                ActorId = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}