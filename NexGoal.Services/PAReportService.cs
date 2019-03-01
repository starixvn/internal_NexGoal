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

        public async Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupAndMember(int actorId)
        {
            var criteria = new GetGroupAndMemberForReportCriteria
            {
                ActorId = actorId
            };
            return await baseService.FetchMultipleAsync<Group, Employee>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PARecordGroupByPeriod>> GetPAReportAsync(GetPARecordMultiPeriodParam param)
        {
            var criteria = new GetPARecordForReportCriteria
            {
                ActorId = param.ActorId,
                PeriodIds = param.PeriodIds,
                GroupIds = param.GroupIds,
                EmployeeIds = param.EmployeeIds,
                IsRevised = param.IsRevised
            };

            var tuple = await reportRepository.GetPAReportAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            var periods = tuple.Item2;

            MappingData(tuple.Item1, performanceGrades);

            var groupRecords = tuple.Item1.GroupBy(r => r.Period.PeriodId)
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

            var emptyPeriods = periods.Where(p => !groupRecords.Any(g => g.PeriodId == p.PeriodId))
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
                    if (process.PerformanceRecord != null)
                    {
                        process.PerformanceRecord.Creator = process.Evaluator;
                        process.PerformanceRecord.GradeId = cryptoService.Decrypt(process.PerformanceRecord.GradeId);
                        int.TryParse(process.PerformanceRecord.GradeId, out gradeId);
                        process.PerformanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                    }
                }
            }
        }

        public async Task<PAReportDetail> GetPAReportDetailAsync(int actorId, int paRecordId)
        {
            var paRecord = await paRecordService.GetPARecordByIdAsync(paRecordId, actorId).ConfigureAwait(false);
            var employeeInfo = new Employee();
            if (paRecord != null)
            {
                employeeInfo = await commonService.GetEmployeeByIdAsync(paRecord.Employee.EmployeeId).ConfigureAwait(false);
            }

            var grades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            Tuple<IEnumerable<DisciplineModel>, IEnumerable<HRMInfo>, string> hrmAndPrevGrade = await GetPARecordDetail(actorId, paRecordId).ConfigureAwait(false);

            var previousGrade = hrmAndPrevGrade.Item3;
            previousGrade = cryptoService.Decrypt(previousGrade);
            int.TryParse(previousGrade, out int gradeId);
            var achievements = await GetAchievements(paRecordId).ConfigureAwait(false);

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
            string>> GetPARecordDetail(int actorId, int paRecordId)
        {
            var criteria = new GetDisciplineHRMInfoAndPreviousPAGradeCriteria
            {
                ActorId = actorId,
                PARecordId = paRecordId
            };

            var details = await baseService.FetchMultipleAsync<DisciplineModel, HRMInfo, string>(criteria).ConfigureAwait(false);

            return new Tuple<IEnumerable<DisciplineModel>,
                IEnumerable<HRMInfo>,
                string>(details.Item1,
                details.Item2,
                details.Item3.FirstOrDefault()
                );
        }

        private async Task<IEnumerable<EmployeeNominationModel>> GetAchievements(int paRecordId)
        {
            var criteria = new GetAchievementByEmployeeCriteria
            {
                PARecordId = paRecordId
            };

            var tuple = await baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(criteria).ConfigureAwait(false);

            foreach (EmployeeNominationModel nomination in tuple.Item1)
            {
                nomination.Period = tuple.Item2.FirstOrDefault(x => x.NominationPeriodId == nomination.NominationPeriodId);
            }

            return tuple.Item1;
        }
    }
}