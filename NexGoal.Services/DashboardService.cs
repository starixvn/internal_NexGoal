namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Dashboard;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public class DashboardService : IDashboardService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public DashboardService(
            IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PARecordModel>> GetPARecordAsync()
        {
            var criteria = new GetPARecordCriteria
            {
                EmployeeId = sessionService.EmployeeId
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