namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Report;
    using NexGoal.Models;
    using NexGoal.Models.Common;
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
        private readonly IDisciplineService disciplineService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PAReportService(
            IPAReportRepository reportRepository,
            IPARecordService paRecordService,
            IDisciplineService disciplineService,
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.paRecordService = paRecordService;
            this.reportRepository = reportRepository;
            this.baseService = baseService;
            this.disciplineService = disciplineService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PARecordGroupByPeriod>> GetPAReportAsync(GetPARecordMultiPeriodParam param)
        {
            var criteria = new GetPARecordForReportCriteria
            {
                ActorId = param.ActorId,
                PeriodIds = param.PeriodIds,
                GroupIds = param.GroupIds,
                EmployeeId = param.EmployeeId
            };

            IEnumerable<PARecordModel> performanceAppraisals = await reportRepository.GetPAReportAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            MappingData(performanceAppraisals, performanceGrades);

            var groupRecords = performanceAppraisals.GroupBy(r => r.Period.PeriodId)
                .Select(item => new PARecordGroupByPeriod
                {
                    PeriodId = item.Key,
                    Period = item.First().Period,
                    Groups = item.GroupBy(g => new { g.Employee.GroupCode, g.Employee.GroupName })
                        .Select(group => new PAReportModel
                        {
                            Group = new GroupBaseModel { Code = group.Key.GroupCode, Name = group.Key.GroupName },
                            PARecords = group
                        })
                });

            return groupRecords.OrderByDescending(g => g.PeriodId);
        }

        private void MappingData(
            IEnumerable<PARecordModel> performanceAppraisals,
            IEnumerable<PerformanceGradeModel> performanceGrades)
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
                        process.PerformanceRecord.GradeId = cryptoService.Decrypt(process.PerformanceRecord.GradeId);
                        int.TryParse(process.PerformanceRecord.GradeId, out gradeId);
                        process.PerformanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                    }
                }
            }
        }

        public async Task<PAReportDetail> GetPAReportDetailAsync(GetPADetailParam model)
        {
            var paRecord = await paRecordService.GetPARecordByIdAsync(model.PARecordId, model.ActorId).ConfigureAwait(false);
            var disciplines = await disciplineService.GetEmployeeDisciplineByPeriodAsync(model.EmployeeId, model.PARecordId).ConfigureAwait(false);
            var employeeInfo = await commonService.GetEmployeeByIdAsync(model.EmployeeId).ConfigureAwait(false);
            var grades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            var previousGrade = await paRecordService.GetPreviousPAGrade(model.PeriodId, model.ActorId, model.EmployeeId).ConfigureAwait(false);
            previousGrade = cryptoService.Decrypt(previousGrade);
            int.TryParse(previousGrade, out int gradeId);
            var achievements = await GetAchievements(model.EmployeeId, model.PeriodId).ConfigureAwait(false);

            var paDetail = new PAReportDetail
            {
                Employee = employeeInfo,
                PARecord = paRecord,
                Disciplines = disciplines,
                AllGrades = grades,
                Achievements = achievements,
                PreviousGradeId = previousGrade,
                PreviousGrade = grades.FirstOrDefault(g => g.GradeId == gradeId)
            };

            return paDetail;
        }

        private async Task<IEnumerable<EmployeeNominationModel>> GetAchievements(int employeeId, int periodId)
        {
            var criteria = new GetAchievementByEmployeeCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId
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