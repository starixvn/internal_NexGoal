namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.NominationReport;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;

    public class NominationReportService : INominationReportService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public NominationReportService(IBaseService baseService, ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync()
        {
            var criteria = new GetNominationPeriodsCriteria();

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<NominationModel>> GetNominations(string nominationPeriodIds, string groupIds, string employeeIds, int? statusId, int loggedEmployeeId)
        {
            var criteria = new GetNominationsCriteria
            {
                NominationPeriodIds = nominationPeriodIds,
                GroupIds = groupIds,
                EmployeeIds = employeeIds,
                StatusId = statusId,
                LoggedEmployeeId = loggedEmployeeId
            };

            var result = await baseService.FetchMultipleAsync<NominationModel, PerformanceRecordModel>(criteria).ConfigureAwait(false);

            var nominations = result.Item1;
            var performanceRecords = result.Item2;

            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            MappingData(performanceRecords, employees, nominations);

            return nominations.OrderBy(x => x.Nominee.NickName);
        }

        private static void MappingData(IEnumerable<PerformanceRecordModel> performanceRecords, IEnumerable<EmployeeBaseModel> employees, IEnumerable<NominationModel> nominations)
        {
            foreach (var record in performanceRecords)
            {
                record.Creator = employees.FirstOrDefault(x => x.EmployeeId == record.CreatorId);
            }

            foreach (var nomination in nominations)
            {
                nomination.Nominee = employees.FirstOrDefault(x => x.EmployeeId == nomination.NomineeId);
                nomination.Nominators = performanceRecords.Where(x => x.EmployeeIds == nomination.NomineeId.ToString() && x.NominationPeriodId == nomination.NominationPeriodId);
            }
        }
    }
}