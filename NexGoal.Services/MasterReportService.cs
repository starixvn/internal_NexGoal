namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.MasterReport;
    using NexGoal.Models.Common;
    using NexGoal.Models.MasterReport;

    public class MasterReportService : IMasterReportService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public MasterReportService(IBaseService baseService, ICryptoService cryptoService, ICommonService commonService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriods()
        {
            return await baseService.GetDataAsync<PeriodBaseModel>(new GetPeriodsCriteria()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MasterReportModel>> GetRecords(int periodId, string groupIds, string employeeIds)
        {
            var result = await baseService.FetchMultipleAsync<MasterReportModel, AwardModel>(new GetRecordsCriteria
            {
                PeriodId = periodId,
                GroupIds = groupIds,
                EmployeeIds = employeeIds
            }).ConfigureAwait(false);

            var performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);
            var focusEvaluationLevels = await commonService.GetReferenceResourcesByNamesAsync(
                new[] { ReferenceResource.FocusEvaluationLevel }).ConfigureAwait(false);

            var records = result.Item1;
            var awards = result.Item2;

            foreach (var record in records)
            {
                record.IsReadyPromotion = cryptoService.Decrypt(record.IsReadyPromotion);
                record.Contribution = focusEvaluationLevels.FirstOrDefault(x => x.ResourceId.ToString() == cryptoService.Decrypt(record.Contribution))?.Value;
                record.BonusGiven = focusEvaluationLevels.FirstOrDefault(x => x.ResourceId.ToString() == cryptoService.Decrypt(record.BonusGiven))?.Value;

                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                record.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId)?.Name;

                record.OriginalGradeId = cryptoService.Decrypt(record.OriginalGradeId);
                int.TryParse(record.OriginalGradeId, out gradeId);
                record.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId)?.Name;

                record.Awards = awards.Where(x => x.NomineeId == record.EmployeeId);
            }

            return records;
        }
    }
}