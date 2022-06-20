namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Repositories;

    public class EmployeeNominationService : IEmployeeNominationService
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeNominationRepository nominationRepository;
        private readonly ICommonService commonService;

        public EmployeeNominationService(
            IBaseService baseService,
            IEmployeeNominationRepository nominationRepository,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.nominationRepository = nominationRepository;
            this.commonService = commonService;
        }

        public async Task<CommandResult> CreateAsync(CreateEmployeeNominationModel model)
        {
            var criteria = new CreateEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                Comment = commonService.RemoveInvalidXmlChars(model.Comment)
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            commandResult.ObjectIds = criteria.PerformanceRecordId.ToString();

            return commandResult;
        }

        public async Task<CommandResult> EditAsync(EditEmployeeNominationModel model)
        {
            var criteria = new EditEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                Comment = commonService.RemoveInvalidXmlChars(model.Comment),
                PerformanceRecordId = model.PerformanceRecordId,
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<CommandResult> DeleteAsync(int performanceRecordId, int deletedById)
        {
            var criteria = new DeleteNominationCriteria
            {
                PerformanceRecordId = performanceRecordId,
                DeletedById = deletedById
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominatorAsync(int nominatorId)
        {
            var criteria = new GetNominationPeriodsForNominatorCriteria
            {
                NominatorId = nominatorId
            };

            return await baseService.GetDataAsync<NominationPeriodModel>(criteria);
        }

        public async Task<NominationPeriodModel> GetCurrentNominationPeriodForComitteeAsync(int committeeId, bool isQuarter)
        {
            var criteria = new GetNominationPeriodsForComitteeCriteria
            {
                CommitteeId = committeeId,
                IsQuarter = isQuarter
            };

            return await baseService.GetDataEntityAsync<NominationPeriodModel>(criteria);
        }

        public async Task<PerformanceRecord> GetNominationByRecordIdAsync(int performanceRecordId)
        {
            var criteria = new GetNominationByRecordIdCriteria
            {
                RecordId = performanceRecordId
            };

            return await nominationRepository.GetNominationByRecordIdAsync(criteria);
        }

        public async Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(int employeeId)
        {
            var criteria = new GetNominationResultCriteria
            {
                EmployeeId = employeeId
            };

            return await nominationRepository.GetNominationResultAsync(criteria);
        }

        public async Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationResultAsync(
            int actorId,
            int? employeeId = null,
            int? year = null)
        {
            var criteria = new GetMemberNominationResultCriteria
            {
                ActorId = actorId,
                Year = year,
                EmployeeId = employeeId
            };

            return await nominationRepository.GetNominationResultAsync(criteria);
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberNominationAsync(int nominatorId)
        {
            var criteria = new GetMemberNominationCriteria
            {
                ActorId = nominatorId
            };
            IEnumerable<PerformanceRecord> records = await nominationRepository.GetMemberNominationAsync(criteria);
            IEnumerable<PerformanceRecord> performanceRecords = GroupDataMemberPerformance(records);

            return performanceRecords;
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberNominationByIdAsync(int nominatorId, int memberId)
        {
            var criteria = new GetMemberNominationByIdCriteria
            {
                ActorId = nominatorId,
                MemberId = memberId
            };
            IEnumerable<PerformanceRecord> performanceRecords = await nominationRepository.GetMemberNominationAsync(criteria);

            return performanceRecords;
        }

        public async Task<IEnumerable<NominationModel>> GetNominationsByPeriodIdsAsync(string nominationPeriodIds, int? statusId = null)
        {
            var criteria = new GetNominationsByPeriodIdsCriteria
            {
                NominationPeriodIds = nominationPeriodIds,
                StatusId = statusId
            };

            IEnumerable<NominationModel> nominations = await nominationRepository.GetNominationByPeriodIdsAsync(criteria);

            return nominations.OrderBy(x => x.Nominee.NickName);
        }

        public async Task<CommandResult> EditStatusAsync(int nominationId, int statusId, int updatedById)
        {
            var criteria = new EditNominationStatusCriteria
            {
                NominationId = nominationId,
                StatusId = statusId,
                UpdatedById = updatedById
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        private static IEnumerable<PerformanceRecord> GroupDataMemberPerformance(IEnumerable<PerformanceRecord> performanceRecords)
        {
            var records = performanceRecords.Where(x => x.TypeId == (int)PerformanceRecordType.Nomination).ToList();
            var systemNominations = performanceRecords.Where(x => x.TypeId == (int)PerformanceRecordType.SystemNomination);

            foreach (PerformanceRecord nomination in systemNominations)
            {
                if (!records.Any(x => x.NominationPeriod.NominationPeriodId == nomination.NominationPeriod.NominationPeriodId
                    && x.NominationStatusId == nomination.NominationStatusId
                    && x.TypeId == (int)PerformanceRecordType.SystemNomination))
                {
                    records.Add(nomination);
                }
                else
                {
                    int index = records.FindIndex(x => x.NominationPeriod.NominationPeriodId == nomination.NominationPeriod.NominationPeriodId
                        && x.NominationStatusId == nomination.NominationStatusId
                        && x.TypeId == (int)PerformanceRecordType.SystemNomination);
                    records[index].Employees = records[index].Employees.Concat(nomination.Employees);
                }
            }

            return records.OrderByDescending(x => x.CreatedDate);
        }

        public async Task<IEnumerable<TemplateKPINominationInfoModel>> GetTemplateKIPNominationAsync()
        {
            var criteria = new GetTemplateKIPNominationCriteria();

            var templateKPIs = await baseService.GetDataAsync<TemplateKIPNominationModel>(criteria);

            return templateKPIs.GroupBy(p => new { p.KPIGroupId, p.Name }).Select(k => new TemplateKPINominationInfoModel
            {
                KPIGroupId = k.Key.KPIGroupId,
                Name = k.Key.Name,
                KPIs = k.Select(kpi => new KPIModel { KPIId = kpi.KPIId, KPIName = kpi.KPIName }),
            });
        }
    }
}