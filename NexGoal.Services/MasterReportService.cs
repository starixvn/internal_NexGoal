namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.MasterReport;
    using NexGoal.Models.Common;
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

        public MasterReportService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService,
            IHostingEnvironment hostingEnvironment)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.hostingEnvironment = hostingEnvironment;
        }

        private async Task<IEnumerable<MasterReportModel>> GetRecords(int periodId, string groupIds, string employeeIds)
        {
            var result = await baseService.FetchMultipleAsync<MasterReportModel, AwardModel, AssessmentModel>(new GetRecordsCriteria
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
            var assessments = result.Item3;

            foreach (var assessment in assessments)
            {
                assessment.GradeId = cryptoService.Decrypt(assessment.GradeId);
                assessment.Grade = performanceGrades.FirstOrDefault(x => x.GradeId.ToString() == assessment.GradeId)?.Name;
            }

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
                int.TryParse(record.OriginalGradeId, out int originalGradeId);
                record.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == originalGradeId)?.Name;

                record.Awards = awards.Where(x => x.NomineeId == record.EmployeeId);
                record.Assessments = assessments.Where(x => x.EmployeeId == record.EmployeeId);
            }

            return records;
        }

        public async Task<byte[]> ExportAsync(int periodId, string groupIds, string employeeIds)
        {
            var records = await GetRecords(periodId, groupIds, employeeIds).ConfigureAwait(false);
            var groupedRecord = records.OrderBy(x => x.GroupName).ThenBy(x => x.NickName)
                .GroupBy(x => new { x.GroupId, x.GroupName }, (key, elements) => new { Key = key, Records = elements });

            var gradeConversions = await commonService.GetPerformanceGradeConversionAsync().ConfigureAwait(false);

            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));
            using (var package = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                // write data
                var currentRow = 4;
                foreach (var group in groupedRecord)
                {
                    var groupCell = worksheet.Cells[currentRow, 1, currentRow, 19];
                    groupCell.Merge = true;
                    groupCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                    groupCell.Style.Font.Bold = true;
                    groupCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    groupCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 197, 197));
                    groupCell.Value = group.Key.GroupName;
                    currentRow++;

                    var rowIndex = 1;

                    foreach (var record in group.Records)
                    {
                        WriteRecord(worksheet, record, gradeConversions, ref currentRow, ref rowIndex);
                    }
                }

                return package.GetAsByteArray();
            }
        }

        private static void WriteRecord(ExcelWorksheet worksheet, MasterReportModel record, IEnumerable<PerformanceGradeConversionModel> gradeConversions, ref int currentRow, ref int rowIndex)
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

            var jobTitleCell = worksheet.Cells[currentRow, 3, toRow, 3];
            jobTitleCell.Merge = merge;
            jobTitleCell.Value = record.JobTitle;

            var paCell = worksheet.Cells[currentRow, 4, toRow, 4];
            paCell.Merge = merge;
            WritePA(paCell, record);

            var selfCell = worksheet.Cells[currentRow, 5, toRow, 5];
            selfCell.Merge = merge;
            selfCell.Style.WrapText = true;
            WriteAssessment(selfCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.Self));

            var pmCell = worksheet.Cells[currentRow, 6, toRow, 6];
            pmCell.Merge = merge;
            WriteAssessment(pmCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.ProjectManager));

            var evaluatorCell = worksheet.Cells[currentRow, 7, toRow, 7];
            evaluatorCell.Merge = merge;
            WriteAssessment(evaluatorCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.Evaluator));

            var reviewerCell = worksheet.Cells[currentRow, 8, toRow, 8];
            reviewerCell.Merge = merge;
            WriteAssessment(reviewerCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.Reviewer));

            var actualWDCell = worksheet.Cells[currentRow, 9, toRow, 9];
            actualWDCell.Merge = merge;
            actualWDCell.Value = Math.Round(record.TotalSpentTime * 100 / (record.ActualWorkingDay * 8));

            var workingDayCell = worksheet.Cells[currentRow, 10, toRow, 10];
            workingDayCell.Merge = merge;
            workingDayCell.Value = Math.Round(record.TotalSpentTime * 100 / (record.WorkingDay * 8));

            var avgPMGradeWithoutNACell = worksheet.Cells[currentRow, 11, toRow, 11];
            avgPMGradeWithoutNACell.Merge = merge;
            avgPMGradeWithoutNACell.Value =
                CalculateAverageGrade(record.Assessments.Where(x => x.ProcessId == PerformanceProcess.ProjectManager && !string.IsNullOrWhiteSpace(x.GradeId)), gradeConversions);

            var avgPMGradeWithNACell = worksheet.Cells[currentRow, 12, toRow, 12];
            avgPMGradeWithNACell.Merge = merge;
            avgPMGradeWithNACell.Value = CalculateAverageGrade(record.Assessments.Where(x => x.ProcessId == PerformanceProcess.ProjectManager), gradeConversions);

            WritePAFocus(worksheet, currentRow, toRow, record, merge);

            var currentObjectiveCell = worksheet.Cells[currentRow, 16, toRow, 16];
            currentObjectiveCell.Merge = merge;
            currentObjectiveCell.Value = record.CurrentObjective;

            var nextObjectiveCell = worksheet.Cells[currentRow, 17, toRow, 17];
            nextObjectiveCell.Merge = merge;
            nextObjectiveCell.Value = record.NextObjective;

            var rowPosition = currentRow;
            foreach (var award in record.Awards)
            {
                worksheet.Cells[rowPosition, 18].Value = award.Title;
                var statusCell = worksheet.Cells[rowPosition, 19];
                statusCell.Value = award.Status;
                statusCell.Style.Font.Color.SetColor(GetNominationStatusFontColor(award.StatusId));
                rowPosition++;
            }

            rowIndex++;
            currentRow = toRow + 1;
        }

        private static Color GetNominationStatusFontColor(NominationStatus statusId)
        {
            switch (statusId)
            {
                case NominationStatus.Shortlisted:
                    return Color.FromArgb(229, 97, 0);

                case NominationStatus.Winner:
                    return Color.FromArgb(20, 130, 213);

                default: return Color.FromArgb(79, 158, 0);
            }
        }

        private static void WritePAFocus(ExcelWorksheet worksheet, int currentRow, int toRow, MasterReportModel record, bool merge)
        {
            var backgroundColor = Color.Empty;
            var textColor = Color.Empty;

            var hasReadyPromotionSelect = bool.TryParse(record.IsReadyPromotion, out bool isReadyPromotion);

            if ((record.ContributionId + record.BonusGivenId) > 8 && hasReadyPromotionSelect)
            {
                backgroundColor = isReadyPromotion ? Color.FromArgb(89, 176, 59) : Color.FromArgb(104, 169, 189);
                textColor = Color.White;
            }
            else
            {
                if ((record.ContributionId + record.BonusGivenId) < 4)
                {
                    backgroundColor = Color.FromArgb(255, 95, 95);
                    textColor = Color.White;
                }
            }

            var contributionCell = worksheet.Cells[currentRow, 13, toRow, 13];
            contributionCell.Merge = merge;
            contributionCell.Value = record.Contribution;

            var bonusGivenCell = worksheet.Cells[currentRow, 14, toRow, 14];
            bonusGivenCell.Merge = merge;
            bonusGivenCell.Value = record.BonusGiven;

            var readyPromotionCell = worksheet.Cells[currentRow, 15, toRow, 15];
            readyPromotionCell.Merge = merge;
            if (hasReadyPromotionSelect)
            {
                readyPromotionCell.Value = isReadyPromotion ? "Yes" : "No";
            }

            if (backgroundColor != Color.Empty)
            {
                bonusGivenCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                bonusGivenCell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                contributionCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                contributionCell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                readyPromotionCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                readyPromotionCell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
            }

            if (textColor != Color.Empty)
            {
                bonusGivenCell.Style.Font.Color.SetColor(textColor);
                contributionCell.Style.Font.Color.SetColor(textColor);
                readyPromotionCell.Style.Font.Color.SetColor(textColor);
            }
        }

        private static float CalculateAverageGrade(IEnumerable<AssessmentModel> assessments, IEnumerable<PerformanceGradeConversionModel> gradeConversions)
        {
            var points = assessments.Select(x =>
            {
                int.TryParse(x.GradeId, out int gradeId);
                var conversion = gradeConversions.FirstOrDefault(g => g.GradeId == gradeId);

                return conversion != null ? conversion.Point : 0f;
            });

            return points.Any() ? points.Average() : 0f;
        }

        private static void WriteAssessment(ExcelRange selfCell, IEnumerable<AssessmentModel> assessments)
        {
            var richTextCollection = selfCell.RichText;

            foreach (var assessment in assessments)
            {
                var assessorText = richTextCollection.Add(assessment.NickName);
                assessorText.Bold = true;

                richTextCollection.Add(Environment.NewLine);

                var gradeText = richTextCollection.Add($"Grade: {(string.IsNullOrWhiteSpace(assessment.Grade) ? "N/A" : assessment.Grade)} ");
                gradeText.Bold = false;

                richTextCollection.Add(Environment.NewLine);

                if (!string.IsNullOrWhiteSpace(assessment.Comment))
                {
                    richTextCollection.Add(assessment.Comment);
                }

                richTextCollection.Add(Environment.NewLine);
                richTextCollection.Add(Environment.NewLine);
            }
        }

        private static void WritePA(ExcelRange paCell, MasterReportModel record)
        {
            var richTextCollection = paCell.RichText;
            var assessorText = richTextCollection.Add(record.Assessments.FirstOrDefault(x => x.ProcessId == PerformanceProcess.Reviewer)?.NickName);
            assessorText.Bold = true;

            richTextCollection.Add(Environment.NewLine);

            var gradeText = richTextCollection.Add($"Grade: {(string.IsNullOrWhiteSpace(record.Grade) ? "N/A" : record.Grade)} ");
            gradeText.Bold = false;

            if (record.GradeId != record.OriginalGradeId)
            {
                int.TryParse(record.GradeId, out int gradeId);
                int.TryParse(record.OriginalGradeId, out int originalGradeId);

                var icon = "";
                var color = Color.Black;
                if (gradeId > originalGradeId)
                {
                    icon = "▲";
                    color = Color.Green;
                }
                else
                {
                    if (gradeId < originalGradeId)
                    {
                        icon = "▼";
                        color = Color.Red;
                    }
                }

                var iconText = richTextCollection.Add(icon);
                iconText.Bold = true;
                iconText.Color = color;
            }

            if (!string.IsNullOrWhiteSpace(record.Comment))
            {
                richTextCollection.Add(Environment.NewLine);

                var commentText = richTextCollection.Add(record.Comment);
                commentText.Bold = false;
                commentText.Color = Color.Black;
            }
        }
    }
}