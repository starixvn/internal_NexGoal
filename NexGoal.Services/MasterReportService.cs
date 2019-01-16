namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.MasterReport;
    using NexGoal.Models.Common;
    using NexGoal.Models.MasterReport;
    using OfficeOpenXml;

    public class MasterReportService : IMasterReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\MasterReport.xlsx";

        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly IHostingEnvironment hostingEnvironment;

        public MasterReportService(IBaseService baseService, ICryptoService cryptoService, ICommonService commonService, IHostingEnvironment hostingEnvironment)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.hostingEnvironment = hostingEnvironment;
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

        public async Task<byte[]> ExportAsync(int periodId, string groupIds, string employeeIds)
        {
            var records = await GetRecords(periodId, groupIds, employeeIds).ConfigureAwait(false);

            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));

            using (var package = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                // write data
                var rowIndex = 1;
                var currentRow = 6;
                foreach (var record in records)
                {
                    worksheet.Cells[currentRow, 1].Value = rowIndex;
                    worksheet.Cells[currentRow, 2].Value = record.NickName;
                    worksheet.Cells[currentRow, 3].Value = record.FullName;
                    worksheet.Cells[currentRow, 4].Value = record.JobTitle;
                    worksheet.Cells[currentRow, 5].Value = record.GroupName;

                    rowIndex++;
                    currentRow++;
                }

                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();

                return package.GetAsByteArray();
            }
        }
    }
}