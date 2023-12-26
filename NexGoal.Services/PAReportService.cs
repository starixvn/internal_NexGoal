namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Report;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;
    using NexGoal.Repositories;

    public class PAReportService : IPAReportService
    {
        private readonly IPAReportRepository reportRepository;
        private readonly IPARecordService paRecordService;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PAReportService(
            IPAReportRepository reportRepository,
            IPARecordService paRecordService,
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.paRecordService = paRecordService;
            this.reportRepository = reportRepository;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupAndMemberAsync(int actorId)
        {
            var criteria = new GetGroupAndMemberForReportCriteria
            {
                ActorId = actorId
            };
            return await baseService.FetchMultipleAsync<Group, Employee>(criteria);
        }

        public async Task<IEnumerable<PARecordGroupByPeriod>> GetPAReportAsync(GetPARecordMultiPeriodParam param)
        {
            var criteria = new GetPARecordForReportCriteria
            {
                ActorId = param.ActorId,
                PeriodIds = param.PeriodIds,
                GroupIds = param.GroupIds,
                EmployeeIds = param.EmployeeIds,
                IsRevised = param.IsRevised,
                IsExportExcel = param.IsExportExcel
            };

            var (periods, records) = param.IsExportExcel
                ? await reportRepository.GetDataForExportPAReportAsync(criteria) : await reportRepository.GetPAReportAsync(criteria);
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();

            commonService.MappingGrade(records, performanceGrades);

            var groupRecords = records.GroupBy(r => r.PeriodId)
                .Select(item => new PARecordGroupByPeriod
                {
                    PeriodId = item.Key,
                    Period = periods.FirstOrDefault(p => p.PeriodId == item.Key),
                    Groups = item.GroupBy(g => new { g.Employee.GroupCode, g.Employee.GroupName })
                        .Select(group => new PAReportModel
                        {
                            Group = new Group { Code = group.Key.GroupCode, Name = group.Key.GroupName },
                            PARecords = group
                        })
                }).ToList();

            var emptyPeriods = periods.Where(p => !groupRecords.Exists(g => g.PeriodId == p.PeriodId))
                .Select(x => new PARecordGroupByPeriod
                {
                    PeriodId = x.PeriodId,
                    Period = x
                });

            if (emptyPeriods.Any())
            {
                groupRecords.AddRange(emptyPeriods);
            }

            return groupRecords.OrderByDescending(g => g.Period.EndDate);
        }

        public async Task<PAReportDetail> GetPAReportDetailAsync(int actorId, int paRecordId)
        {
            var paRecord = await paRecordService.GetPARecordByIdAsync(paRecordId, actorId);
            var employeeInfo = new Employee();
            if (paRecord != null)
            {
                employeeInfo = await commonService.GetEmployeeByIdAsync(paRecord.Employee.EmployeeId);
            }

            var grades = await commonService.GetPerformanceGradeAsync();
            var (disciplines, hrmInfos, previousGrade) = await GetPARecordDetailAsync(actorId, paRecordId);

            previousGrade = cryptoService.Decrypt(previousGrade);
            _ = int.TryParse(previousGrade, out int gradeId);
            var achievements = await GetAchievementAsync(paRecordId);
            var paDetails = new PAReportDetail
            {
                Employee = employeeInfo,
                PARecord = paRecord,
                HRM = hrmInfos,
                Disciplines = disciplines,
                AllGrades = grades,
                Achievements = achievements,
                PreviousGradeId = previousGrade,
                PreviousGrade = grades.FirstOrDefault(g => g.GradeId == gradeId)
            };

            return paDetails;
        }

        private async Task<ValueTuple<IEnumerable<DisciplineModel>, IEnumerable<HRMInfo>, string>> GetPARecordDetailAsync(int actorId, int paRecordId)
        {
            var criteria = new GetDisciplineHRMInfoAndPreviousPAGradeCriteria
            {
                ActorId = actorId,
                PARecordId = paRecordId
            };
            var (disciplines, hrmInfos, previousGrades) = await baseService.FetchMultipleAsync<DisciplineModel, HRMInfo, string>(criteria);

            return new (disciplines, hrmInfos, previousGrades.FirstOrDefault());
        }

        private async Task<IEnumerable<EmployeeNominationModel>> GetAchievementAsync(int paRecordId)
        {
            var criteria = new GetAchievementByEmployeeCriteria
            {
                PARecordId = paRecordId
            };
            var (nominations, periods) = await baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(criteria);
            foreach (var nomination in nominations)
            {
                nomination.Period = periods.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            return nominations;
        }
    }
}