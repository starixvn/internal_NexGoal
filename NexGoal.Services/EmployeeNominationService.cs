namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;

    public class EmployeeNominationService : IEmployeeNominationService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public EmployeeNominationService(
            IBaseService baseService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(int nominatorId)
        {
            var criteria = new GetNominationPeriodsForNominatorCriteria
            {
                NominatorId = nominatorId
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<NominationPeriodModel> GetCurrentNominationPeriodForComittee(int committeeId, bool isQuarter)
        {
            var criteria = new GetNominationPeriodsForComitteeCriteria
            {
                CommitteeId = committeeId,
                IsQuarter = isQuarter
            };

            return await baseService.GetDataEntityAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateAsync(CreateEmployeeNominationModel model)
        {
            var criteria = new CreateEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                Comment = model.Comment
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            commandResult.ObjectIds = criteria.PerformanceRecordId.ToString();

            return commandResult;
        }

        public async Task<PerformanceRecordModel> GetNominationByRecordIdAsync(int performanceRecordId)
        {
            var criteria = new GetNominationByRecordIdCriteria
            {
                RecordId = performanceRecordId
            };

            var result = await baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            var performanceRecord = result.Item1.FirstOrDefault();

            if (performanceRecord != null)
            {
                var nominationPeriod = result.Item2.FirstOrDefault();

                performanceRecord.Creator = await commonService.GetEmployeeByIdAsync(performanceRecord.CreatorId).ConfigureAwait(false);
                performanceRecord.NominationPeriod = nominationPeriod;

                var nominee = await commonService.GetEmployeeByIdAsync(int.Parse(performanceRecord.EmployeeIds)).ConfigureAwait(false);
                performanceRecord.Employees = new List<EmployeeBaseModel> { nominee };
            }

            return performanceRecord;
        }

        public async Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationsAsync(int nominatorId, int? memberId)
        {
            var criteria = new GetMemberNominationCriteria
            {
                ActorId = nominatorId,
                MemberId = memberId
            };

            var result = await baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            var performanceRecords = GroupDataMemberPerformance(result.Item1);
            var nominationPeriods = result.Item2;

            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);
            employees = employees.Where(x => performanceRecords.Any(r => r.CreatorId == x.EmployeeId || r.EmployeeIds.Split(',').Select(int.Parse).Any(e => e == x.EmployeeId)));

            MappingData(performanceRecords, employees, nominationPeriods);

            return performanceRecords;
        }

        public async Task<IEnumerable<NominationModel>> GetNominationsByPeriodIdsAsync(string nominationPeriodIds, int? statusId)
        {
            var criteria = new GetNominationsByPeriodIdsCriteria
            {
                NominationPeriodIds = nominationPeriodIds,
                StatusId = statusId
            };

            var result = await baseService.FetchMultipleAsync<NominationModel, PerformanceRecordModel>(criteria).ConfigureAwait(false);

            var nominations = result.Item1;
            var performanceRecords = result.Item2;

            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);

            MappingData(performanceRecords, employees, nominations);

            return nominations.OrderBy(x => x.Nominee.NickName);
        }

        public async Task<CommandResult> EditAsync(EditEmployeeNominationModel model)
        {
            var criteria = new EditEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                Comment = model.Comment,
                PerformanceRecordId = model.PerformanceRecordId,
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> EditStatusAsync(int nominationId, int statusId, int updatedById)
        {
            var criteria = new EditNominationStatusCriteria
            {
                NominationId = nominationId,
                StatusId = statusId,
                UpdatedById = updatedById
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> DeleteAsync(int performanceRecordId, int deletedById)
        {
            var criteria = new DeleteNominationCriteria
            {
                PerformanceRecordId = performanceRecordId,
                DeletedById = deletedById
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        private static void MappingData(
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<EmployeeBaseModel> employees,
            IEnumerable<NominationModel> nominations)
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

        private static void MappingData(
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<EmployeeBaseModel> employees,
            IEnumerable<NominationPeriodModel> nominationPeriods)
        {
            foreach (var record in performanceRecords)
            {
                var employeeIds = record.EmployeeIds.Split(',').Select(int.Parse);
                record.Employees = employees.Where(x => employeeIds.Any(e => e == x.EmployeeId));
                record.Creator = record.CreatorId == Constants.NexGoalSystemId ?
                    NexGoalSystemInfo.NexGoal : employees.FirstOrDefault(x => x.EmployeeId == record.CreatorId);

                record.NominationPeriod = nominationPeriods.FirstOrDefault(x => x.NominationPeriodId == record.NominationPeriodId);
            }
        }

        private static IEnumerable<PerformanceRecordModel> GroupDataMemberPerformance(IEnumerable<PerformanceRecordModel> performanceRecords)
        {
            var records = performanceRecords.Where(x => x.TypeId == (int)PerformanceRecordType.Nomination).ToList();
            var systemNominations = performanceRecords.Where(x => x.TypeId == (int)PerformanceRecordType.SystemNomination);

            foreach (PerformanceRecordModel nomination in systemNominations)
            {
                if (!records.Any(x => x.NominationPeriodId == nomination.NominationPeriodId && x.NominationStatusId == nomination.NominationStatusId
                    && x.TypeId == (int)PerformanceRecordType.SystemNomination))
                {
                    records.Add(nomination);
                }
                else
                {
                    int index = records.FindIndex(x => x.NominationPeriodId == nomination.NominationPeriodId && x.NominationStatusId == nomination.NominationStatusId
                        && x.TypeId == (int)PerformanceRecordType.SystemNomination);
                    records[index].EmployeeIds = $"{records[index].EmployeeIds},{nomination.EmployeeIds}";
                }
            }

            return records.OrderBy(x => x.StatusId == PerformanceRecordStatus.Draft ? 1 : 2).ThenByDescending(x => x.CreatedDate);
        }
    }
}