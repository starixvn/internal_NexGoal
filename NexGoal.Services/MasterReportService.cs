namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.MasterReport;
    using NexGoal.Models.MasterReport;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

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
                var contribution = focusEvaluationLevels.FirstOrDefault(x => x.ResourceId.ToString() == cryptoService.Decrypt(record.Contribution));
                record.Contribution = contribution?.Value;
                record.ContributionId = contribution?.ResourceId;

                var bonusGiven = focusEvaluationLevels.FirstOrDefault(x => x.ResourceId.ToString() == cryptoService.Decrypt(record.BonusGiven));
                record.BonusGivenId = bonusGiven?.ResourceId;
                record.BonusGiven = bonusGiven?.Value;

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
            records = records.OrderBy(x => x.GroupName).ThenBy(x => x.NickName);

            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));
            using (var package = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                // write data
                var rowIndex = 1;
                var currentRow = 6;
                foreach (var record in records)
                {
                    var numberOfAwards = record.Awards.Count();
                    var toRow = numberOfAwards > 0 ? currentRow + numberOfAwards - 1 : currentRow;
                    var merge = currentRow != toRow;

                    var indexCell = worksheet.Cells[currentRow, 1, toRow, 1];
                    indexCell.Merge = merge;
                    indexCell.Value = rowIndex;

                    var nickNameCell = worksheet.Cells[currentRow, 2, toRow, 2];
                    nickNameCell.Merge = merge;
                    nickNameCell.Value = record.NickName;

                    var fullNameCell = worksheet.Cells[currentRow, 3, toRow, 3];
                    fullNameCell.Merge = merge;
                    fullNameCell.Value = record.FullName;

                    var jobTitleCell = worksheet.Cells[currentRow, 4, toRow, 4];
                    jobTitleCell.Merge = merge;
                    jobTitleCell.Value = record.JobTitle;

                    var groupNameCell = worksheet.Cells[currentRow, 5, toRow, 5];
                    groupNameCell.Merge = merge;
                    groupNameCell.Value = record.GroupName;

                    var gradeCell = worksheet.Cells[currentRow, 7, toRow, 7];
                    gradeCell.Merge = merge;
                    gradeCell.Value = record.Grade;

                    var contributionCell = worksheet.Cells[currentRow, 9, toRow, 9];
                    contributionCell.Merge = merge;
                    contributionCell.Value = record.Contribution;

                    var bonusGivenCell = worksheet.Cells[currentRow, 10, toRow, 10];
                    bonusGivenCell.Merge = merge;
                    bonusGivenCell.Value = record.Contribution;

                    var readyPromotionCell = worksheet.Cells[currentRow, 11, toRow, 11];
                    readyPromotionCell.Merge = merge;
                    readyPromotionCell.Value = record.IsReadyPromotion == "true" ? "Yes" : "No";

                    var currentObjectiveCell = worksheet.Cells[currentRow, 13, toRow, 13];
                    currentObjectiveCell.Merge = merge;
                    currentObjectiveCell.Value = record.CurrentObjective;

                    var nextObjectiveCell = worksheet.Cells[currentRow, 14, toRow, 14];
                    nextObjectiveCell.Merge = merge;
                    nextObjectiveCell.Value = record.NextObjective;

                    var rowPosition = currentRow;
                    foreach (var award in record.Awards)
                    {
                        worksheet.Cells[rowPosition, 15].Value = award.Title;
                        worksheet.Cells[rowPosition, 16].Value = award.Status;
                        rowPosition++;
                    }

                    rowIndex++;
                    currentRow = toRow + 1;
                }

                return package.GetAsByteArray();
            }
        }
    }
}