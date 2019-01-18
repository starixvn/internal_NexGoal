namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Report;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;
    using NexGoal.Repositories;

    public class PAReportService : IPAReportService
    {
        private readonly IPAReportRepository reportRepository;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PAReportService(
            IPAReportRepository reportRepository,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.reportRepository = reportRepository;
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

            var groupRecords = performanceAppraisals.GroupBy(r => r.Period)
                .Select(item => new PARecordGroupByPeriod
                {
                    Period = item.Key,
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
    }
}