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

        public async Task<CommandResult> CreateEoyAsync(CreateEmployeeNominationModel model)
        {
            foreach (var kpi in model.KpiNominations)
            {
                kpi.Comment = commonService.RemoveInvalidXmlChars(kpi.Comment);
            }

            var criteria = new CreateEmployeeNominationEoyCriteria
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

        public async Task<CommandResult> EditEoyAsync(EditEmployeeNominationModel model)
        {
            foreach (var kpi in model.KpiNominations)
            {
                kpi.Comment = commonService.RemoveInvalidXmlChars(kpi.Comment);
            }

            var criteria = new EditEmployeeNominationEoyCriteria
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

            return await nominationRepository.GetNominationByRecordIdAsync(criteria);
        }

        public async Task<PerformanceRecord> GetNominationByRecordIdEoyAsync(int performanceRecordId)
        {

            var criteria = new GetNominationByRecordIdEoyCriteria
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
            var criteriaEoy = new GetMemberNominationEoyCriteria
            {
                ActorId = nominatorId
            };
            var records = await nominationRepository.GetMemberNominationAsync(criteria);
            var performanceRecords = GroupDataMemberPerformance(records);

            var recordEoys = await nominationRepository.GetMemberNominationAsync(criteriaEoy);
            var recordKpiEoys = GroupKPIPerformances(recordEoys);
            var performanceRecordEoys = GroupDataMemberPerformance(recordKpiEoys);

            var selectRecords = performanceRecords.Where(p => !performanceRecordEoys.Any(pn => pn.PerformanceRecordId == p.PerformanceRecordId));

            return performanceRecordEoys.Concat(selectRecords);
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

            var criteriaNew = new GetNominationsByPeriodIdsEoyCriteria
            {
                NominationPeriodIds = nominationPeriodIds,
                StatusId = statusId
            };

            var index = 0;
            IEnumerable<NominationModel> nominations = await nominationRepository.GetNominationByPeriodIdsAsync(criteria);
            IEnumerable<NominationModel> nominationsNew = await nominationRepository.GetNominationByPeriodIdsAsync(criteriaNew);

            foreach (NominationModel nomination in nominations)
            {
                var nominatorNew = nominationsNew.ElementAt(index).Nominators;
                var selected = nomination.Nominators.Where(p => !nominatorNew.Any(pn => pn.PerformanceRecordId == p.PerformanceRecordId));

                nominatorNew = GroupKPIPerformances(nominatorNew);
                nominationsNew.ElementAt(index).Nominators = nominatorNew.Concat(selected).OrderByDescending(x => x.CreatedDate);

                index++;
            }

            return nominationsNew.OrderByDescending(x => x.StatusId).ThenBy(x => x.Nominee.NickName);
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

        public static IEnumerable<PerformanceRecord> GroupKPIPerformances(IEnumerable<PerformanceRecord> performanceRecords)
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