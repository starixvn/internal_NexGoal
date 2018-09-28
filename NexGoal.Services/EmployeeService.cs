namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Employee;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.PARecord;

    public class EmployeeService : IEmployeeService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public EmployeeService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(string keyword, int? groupId, bool? isLocal, bool isDisabled)
        {
            var criteria = new GetEmployeesCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsLocal = isLocal,
                IsDisabled = isDisabled
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataEntityAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetRolesByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<EmployeeRoleModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model)
        {
            var criteria = new EditEmployeeRolesCriteria
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = model.ModifiedBy
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeWorkLoadModel>> GetWorkLoadAsync(int employeeId)
        {
            var criteria = new GetWorkLoadByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<EmployeeWorkLoadModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordModel>> GetPerformanceAppraisalResultAsync(int employeeId)
        {
            var criteria = new GetPerformanceAppraisalResult
            {
                EmployeeId = employeeId
            };

            var paRecords = await baseService.GetDataAsync<PARecordModel>(criteria).ConfigureAwait(false);
            IEnumerable<PeriodBaseModel> periods = await GetPeriodsAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (PARecordModel paRecord in paRecords)
            {
                paRecord.GradeId = cryptoService.Decrypt(paRecord.GradeId);
                int.TryParse(paRecord.GradeId, out int gradeId);
                paRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                paRecord.Period = periods.FirstOrDefault(x => x.PeriodId == paRecord.PeriodId);
            }

            return paRecords;
        }

        private async Task<IEnumerable<PeriodBaseModel>> GetPeriodsAsync()
        {
            var criteria = new GetPeriodsCriteria { IsActive = true };

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }
    }
}