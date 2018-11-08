namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PerformanceRecord;

    public class EmployeeNominationService : IEmployeeNominationService
    {
        private readonly IBaseService baseService;

        private readonly IAvatarService avatarService;

        private readonly ICommonService commonService;

        public EmployeeNominationService(IBaseService baseService, IAvatarService avatarService, ICommonService commonService)
        {
            this.baseService = baseService;
            this.avatarService = avatarService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominator(int nominatorId)
        {
            var criteria = new GetNominationPeriodsForNominatorCriteria
            {
                NominatorId = nominatorId
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> Create(CreateEmployeeNominationModel model)
        {
            var criteria = new CreateEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                Comment = model.Comment
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            commandResult.ObjectId = criteria.PerformanceRecordId;

            return commandResult;
        }

        public async Task<PerformanceRecordModel> GetNominationByRecordIdAsync(int performanceRecordId)
        {
            var criteria = new GetNominationByRecordIdCriteria
            {
                RecordId = performanceRecordId
            };

            var result = await baseService.FetchMultipleAsync<PerformanceRecordModel, NominationModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            var performanceRecord = result.Item1.FirstOrDefault();
            var nomination = result.Item2.FirstOrDefault();
            var nominationPeriod = result.Item3.FirstOrDefault();

            performanceRecord.Creator = await commonService.GetEmployeeByIdAsync(performanceRecord.CreatorId).ConfigureAwait(false);
            performanceRecord.Creator.Avatar = avatarService.GetFilePath(performanceRecord.Creator.Code);

            var nominee = await commonService.GetEmployeeByIdAsync(int.Parse(performanceRecord.EmployeeIds)).ConfigureAwait(false);
            nominee.Avatar = avatarService.GetFilePath(nominee.Code);
            performanceRecord.Employees = new List<EmployeeBaseModel> { nominee };

            nomination.Period = nominationPeriod;
            performanceRecord.Nomination = nomination;

            return performanceRecord;
        }

        public async Task<Tuple<IEnumerable<PerformanceRecordModel>, int>> GetMemberNominationssAsync(int nominatorId, int? memberId, int pageSize, int page)
        {
            var criteria = new GetMemberNominationCriteria
            {
                ActorId = nominatorId,
                MemberId = memberId,
                PageSize = pageSize,
                Page = page
            };

            var result = await baseService.FetchMultipleAsync<PerformanceRecordModel, NominationModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            var performanceRecords = result.Item1;
            var nominations = result.Item2;
            var nominationPeriods = result.Item3;

            var employees = await commonService.GetEmployeesAsync(isEnabled: null).ConfigureAwait(false);
            employees = employees.Where(x => performanceRecords.Any(r => r.CreatorId == x.EmployeeId || r.EmployeeIds.Split(',').Select(int.Parse).Any(e => e == x.EmployeeId)));

            MappingData(performanceRecords, employees, nominations, nominationPeriods);

            return new Tuple<IEnumerable<PerformanceRecordModel>, int>(performanceRecords, criteria.Total);
        }

        private void MappingData(
            IEnumerable<PerformanceRecordModel> performanceRecords,
            IEnumerable<EmployeeBaseModel> employees,
            IEnumerable<NominationModel> nominations,
            IEnumerable<NominationPeriodModel> nominationPeriods)
        {
            foreach (var employee in employees)
            {
                employee.Avatar = avatarService.GetFilePath(employee.Code);
            }

            foreach (var nomination in nominations)
            {
                nomination.Period = nominationPeriods.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            foreach (var record in performanceRecords)
            {
                var employeeIds = record.EmployeeIds.Split(',').Select(int.Parse);
                record.Employees = employees.Where(x => employeeIds.Any(e => e == x.EmployeeId));
                record.Creator = employees.FirstOrDefault(x => x.EmployeeId == record.CreatorId);
                record.Nomination = nominations.FirstOrDefault(x => x.NominationId == record.NominationId);
            }
        }

        public async Task<bool> IsNominator(int actorId)
        {
            var criteria = new CheckIsNominatorCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);
        }
    }
}