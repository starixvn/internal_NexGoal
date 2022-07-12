namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
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
            foreach (var kpi in model.KpiNominations)
            {
                kpi.Comment = commonService.RemoveInvalidXmlChars(kpi.Comment);
            }

            var criteria = new CreateEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                KpiNominations = model.KpiNominations.ToXML().ToString()
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            commandResult.ObjectIds = criteria.PerformanceRecordId.ToString();

            return commandResult;
        }

        public async Task<CommandResult> EditAsync(EditEmployeeNominationModel model)
        {
            foreach (var kpi in model.KpiNominations)
            {
                kpi.Comment = commonService.RemoveInvalidXmlChars(kpi.Comment);
            }

            var criteria = new EditEmployeeNominationCriteria
            {
                NominationPeriodId = model.NominationPeriodId,
                NominatorId = model.NominatorId,
                NomineeId = model.NomineeId,
                PerformanceRecordId = model.PerformanceRecordId,
                KpiNominations = model.KpiNominations.ToXML().ToString()
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

            var record = await nominationRepository.GetMemberNominationAsync(criteria);
            var groupRecord = GroupKPIPerformances(record);

            return groupRecord.FirstOrDefault();
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
            var criteriaNew = new GetMemberNominationNewCriteria
            {
                ActorId = nominatorId
            };
            var records = await nominationRepository.GetMemberNominationAsync(criteria);
            var performanceRecords = GroupDataMemberPerformance(records);

            var recordNews = await nominationRepository.GetMemberNominationAsync(criteriaNew);
            var performanceRecordNews = GroupKPIPerformances(recordNews);

            var selectRecords = performanceRecords.Where(p => !performanceRecordNews.Any(pn => pn.PerformanceRecordId == p.PerformanceRecordId));

            return performanceRecordNews.Concat(selectRecords);
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberNominationByIdAsync(int nominatorId, int memberId)
        {
            var criteria = new GetMemberNominationByIdCriteria
            {
                ActorId = nominatorId,
                MemberId = memberId
            };
            var performanceRecords = await nominationRepository.GetMemberNominationAsync(criteria);
            var performanceRecordGroup = GroupKPIPerformances(performanceRecords);

            return performanceRecordGroup;
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

        private static IEnumerable<PerformanceRecord> GroupKPIPerformances(IEnumerable<PerformanceRecord> performanceRecords)
        {
            return performanceRecords.GroupBy(p => p.PerformanceRecordId).Select(k => new PerformanceRecord
            {
                PerformanceRecordId = k.Key,
                Employees = k.FirstOrDefault().Employees,
                CreatedDate = k.FirstOrDefault().CreatedDate,
                Creator = k.FirstOrDefault().Creator,
                Feedbacks = k.FirstOrDefault().Feedbacks,
                Grade = k.FirstOrDefault().Grade,
                IsAllowEditFeedback = k.FirstOrDefault().IsAllowEditFeedback,
                IsDraft = k.FirstOrDefault().IsDraft,
                IsHost = k.FirstOrDefault().IsHost,
                ModifiedDate = k.FirstOrDefault().ModifiedDate,
                NominationPeriod = k.FirstOrDefault().NominationPeriod,
                NominationStatusId = k.FirstOrDefault().NominationStatusId,
                PASummary = k.FirstOrDefault().PASummary,
                Period = k.FirstOrDefault().Period,
                Projects = k.FirstOrDefault().Projects,
                Reactions = k.FirstOrDefault().Reactions,
                TypeId = k.FirstOrDefault().TypeId,
                RoleId = k.FirstOrDefault().RoleId,
                KPIGroupIds = k.GroupBy(t => t.KPIGroupId).Select(y => new TemplateKPINominationInfoModel
                {
                    KpiGroupId = y.Key,
                    Name = y.FirstOrDefault().Name,
                    Rate = y.FirstOrDefault().Rate,
                    Kpis = y.Select(kpi => new KPIModel { KpiId = kpi.KPIId, Comment = kpi.Comment, KpiName = kpi.KPIName })
                })
            });
        }

        public async Task<IEnumerable<TemplateKPINominationInfoModel>> GetTemplateKIPNominationAsync()
        {
            var criteria = new GetTemplateKIPNominationCriteria();

            var templateKPIs = await baseService.GetDataAsync<TemplateKIPNominationModel>(criteria);

            return templateKPIs.GroupBy(p => new { p.KpiGroupId, p.Name }).Select(k => new TemplateKPINominationInfoModel
            {
                KpiGroupId = k.Key.KpiGroupId,
                Name = k.Key.Name,
                Kpis = k.Select(kpi => new KPIModel { KpiId = kpi.KpiId, KpiName = kpi.KpiName }),
            });
        }
    }
}