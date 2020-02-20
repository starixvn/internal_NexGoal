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
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Report;
    using NexGoal.Repositories;
    using NexGoal.Resources;

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

        public async Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupAndMemberAsync(int actorId)
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

            Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>> tuple = param.IsExportExcel
                ? await reportRepository.GetDataForExportPAReportAsync(criteria) : await reportRepository.GetPAReportAsync(criteria);
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();

            MappingData(tuple.Item2, performanceGrades);

            var groupRecords = tuple.Item2.GroupBy(r => r.PeriodId)
                .Select(item => new PARecordGroupByPeriod
                {
                    PeriodId = item.Key,
                    Period = tuple.Item1.FirstOrDefault(p => p.PeriodId == item.Key),
                    Groups = item.GroupBy(g => new { g.Employee.GroupCode, g.Employee.GroupName })
                        .Select(group => new PAReportModel
                        {
                            Group = new Group { Code = group.Key.GroupCode, Name = group.Key.GroupName },
                            PARecords = group
                        })
                }).ToList();

            var emptyPeriods = tuple.Item1.Where(p => !groupRecords.Any(g => g.PeriodId == p.PeriodId))
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

        private void MappingData(
            IEnumerable<PARecordModel> performanceAppraisals,
            IEnumerable<PerformanceGrade> performanceGrades)
        {
            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
                int.TryParse(performanceAppraisal.GradeId, out int gradeId);
                performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                performanceAppraisal.OriginalGradeId = cryptoService.Decrypt(performanceAppraisal.OriginalGradeId);
                int.TryParse(performanceAppraisal.OriginalGradeId, out gradeId);
                performanceAppraisal.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

                foreach (PAProcessModel process in performanceAppraisal.Processes)
                {
                    process.Grade = GetGradeOfPAProcess(process, performanceGrades);
                    foreach (PerformanceRecord performanceRecord in process.RelatedPerformanceRecords)
                    {
                        performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
                        int.TryParse(performanceRecord.GradeId, out gradeId);
                        performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                    }
                }
            }
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
            Tuple<IEnumerable<DisciplineModel>, IEnumerable<HRMInfo>, string> hrmAndPrevGrade = await GetPARecordDetailAsync(actorId, paRecordId);

            var previousGrade = hrmAndPrevGrade.Item3;
            previousGrade = cryptoService.Decrypt(previousGrade);
            int.TryParse(previousGrade, out int gradeId);
            var achievements = await GetAchievementAsync(paRecordId);
            var paDetails = new PAReportDetail
            {
                Employee = employeeInfo,
                PARecord = paRecord,
                HRM = hrmAndPrevGrade.Item2,
                Disciplines = hrmAndPrevGrade.Item1,
                AllGrades = grades,
                Achievements = achievements,
                PreviousGradeId = previousGrade,
                PreviousGrade = grades.FirstOrDefault(g => g.GradeId == gradeId)
            };

            return paDetails;
        }

        private async Task<Tuple<
            IEnumerable<DisciplineModel>,
            IEnumerable<HRMInfo>,
            string>> GetPARecordDetailAsync(int actorId, int paRecordId)
        {
            var criteria = new GetDisciplineHRMInfoAndPreviousPAGradeCriteria
            {
                ActorId = actorId,
                PARecordId = paRecordId
            };
            var details = await baseService.FetchMultipleAsync<DisciplineModel, HRMInfo, string>(criteria);

            return new Tuple<IEnumerable<DisciplineModel>,
                IEnumerable<HRMInfo>,
                string>(details.Item1,
                details.Item2,
                details.Item3.FirstOrDefault()
                );
        }

        private async Task<IEnumerable<EmployeeNominationModel>> GetAchievementAsync(int paRecordId)
        {
            var criteria = new GetAchievementByEmployeeCriteria
            {
                PARecordId = paRecordId
            };
            var tuple = await baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(criteria);
            foreach (EmployeeNominationModel nomination in tuple.Item1)
            {
                nomination.Period = tuple.Item2.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            return tuple.Item1;
        }

        private string GetGradeOfPAProcess(PAProcessModel process, IEnumerable<PerformanceGrade> performanceGrades)
        {
            process.GradeId = cryptoService.Decrypt(process.GradeId);
            int.TryParse(process.GradeId, out int gradeId);
            PerformanceGrade grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            if (grade != null)
            {
                return grade.Name;
            }

            return !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds) ? ViewLang.NA : string.Empty;
        }
    }
}