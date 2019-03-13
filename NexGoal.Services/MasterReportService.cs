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
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
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

        private const int unsatisfactoryGradeId = 1;

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

        private async Task<IEnumerable<MasterReportModel>> GetRecordsAsync(int periodId, string groupIds, string employeeIds, int employeeId)
        {
            var result = await baseService.FetchMultipleAsync<MasterReportModel, AwardModel, AssessmentModel>(new GetRecordsCriteria
            {
                PeriodId = periodId,
                GroupIds = groupIds,
                EmployeeIds = employeeIds,
                ActorId = employeeId
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

        public async Task<byte[]> ExportAsync(int periodId, string groupIds, string employeeIds, int employeeId)
        {
            var period = await baseService.GetDataEntityAsync<PeriodBaseModel>(new GetPeriodCriteria { PeriodId = periodId }).ConfigureAwait(false);
            var records = await GetRecordsAsync(periodId, groupIds, employeeIds, employeeId).ConfigureAwait(false);
            var groupedRecord = records
                .GroupBy(x => new { x.GroupId, x.GroupName }, (key, elements) => new { Key = key, Records = elements });

            var performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));
            using (var package = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                var periodCell = worksheet.Cells[4, 1, 4, 20];
                periodCell.Merge = true;
                periodCell.Value = period.Name;
                periodCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                periodCell.Style.Font.Bold = true;
                periodCell.Style.Font.Color.SetColor(Color.White);
                periodCell.Style.Fill.PatternType = ExcelFillStyle.Solid;

                var periodBackgroundColor = Color.FromArgb(82, 94, 134);
                periodCell.Style.Fill.BackgroundColor.SetColor(periodBackgroundColor);

                var currentRow = 5;
                foreach (var group in groupedRecord)
                {
                    var groupCell = worksheet.Cells[currentRow, 1, currentRow, 20];
                    groupCell.Merge = true;
                    groupCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                    groupCell.Style.Font.Bold = true;
                    groupCell.Style.Fill.PatternType = ExcelFillStyle.Solid;

                    var groupBackgroundColor = Color.FromArgb(197, 197, 197);
                    groupCell.Style.Fill.BackgroundColor.SetColor(groupBackgroundColor);
                    groupCell.Value = group.Key.GroupName;
                    currentRow++;

                    var rowIndex = 1;

                    foreach (var record in group.Records)
                    {
                        WriteRecord(worksheet, record, performanceGrades, ref currentRow, ref rowIndex);
                    }
                }

                return package.GetAsByteArray();
            }
        }

        private static void WriteRecord(ExcelWorksheet worksheet, MasterReportModel record, IEnumerable<PerformanceGrade> performanceGrades, ref int currentRow, ref int rowIndex)
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

            var toeicCell = worksheet.Cells[currentRow, 4, toRow, 4];
            WriteToeic(toeicCell, record, merge);

            var selfCell = worksheet.Cells[currentRow, 5, toRow, 5];
            selfCell.Merge = merge;
            selfCell.Style.WrapText = true;
            WriteAssessment(selfCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.Self && x.PerformanceRecordId.HasValue));

            var pmCell = worksheet.Cells[currentRow, 6, toRow, 6];
            pmCell.Merge = merge;
            WriteAssessment(pmCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.ProjectManager && x.PerformanceRecordId.HasValue));

            var evaluatorCell = worksheet.Cells[currentRow, 7, toRow, 7];
            evaluatorCell.Merge = merge;
            WriteAssessment(evaluatorCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.Evaluator && x.PerformanceRecordId.HasValue));

            var reviewerCell = worksheet.Cells[currentRow, 8, toRow, 8];
            reviewerCell.Merge = merge;
            WriteAssessment(reviewerCell, record.Assessments.Where(x => x.ProcessId == PerformanceProcess.Reviewer && x.PerformanceRecordId.HasValue));

            var paCell = worksheet.Cells[currentRow, 9, toRow, 9];
            paCell.Merge = merge;
            WritePA(paCell, record);

            var actualWDCell = worksheet.Cells[currentRow, 10, toRow, 10];
            actualWDCell.Merge = merge;
            actualWDCell.Value = Math.Round(record.TotalSpentTime * 100 / (record.ActualWorkingDay * 8));

            var workingDayCell = worksheet.Cells[currentRow, 11, toRow, 11];
            workingDayCell.Merge = merge;
            workingDayCell.Value = Math.Round(record.TotalSpentTime * 100 / (record.WorkingDay * 8));

            var avgPMGradeWithoutNACell = worksheet.Cells[currentRow, 12, toRow, 12];
            avgPMGradeWithoutNACell.Merge = merge;
            avgPMGradeWithoutNACell.Value =
                CalculateAverageGrade(record.Assessments.Where(x => x.ProcessId == PerformanceProcess.ProjectManager && !string.IsNullOrWhiteSpace(x.GradeId)), performanceGrades);

            var avgPMGradeWithNACell = worksheet.Cells[currentRow, 13, toRow, 13];
            avgPMGradeWithNACell.Merge = merge;
            avgPMGradeWithNACell.Value = CalculateAverageGrade(record.Assessments.Where(x => x.ProcessId == PerformanceProcess.ProjectManager), performanceGrades);

            WritePAFocus(worksheet, currentRow, toRow, record, merge);

            var currentObjectiveCell = worksheet.Cells[currentRow, 17, toRow, 17];
            currentObjectiveCell.Merge = merge;
            currentObjectiveCell.Value = record.CurrentObjective;

            var nextObjectiveCell = worksheet.Cells[currentRow, 18, toRow, 18];
            nextObjectiveCell.Merge = merge;
            nextObjectiveCell.Value = record.NextObjective;

            var rowPosition = currentRow;
            foreach (var award in record.Awards)
            {
                worksheet.Cells[rowPosition, 19].Value = award.Title;
                var statusCell = worksheet.Cells[rowPosition, 20];
                statusCell.Value = award.Status;
                statusCell.Style.Font.Color.SetColor(GetNominationStatusFontColor(award.StatusId));
                rowPosition++;
            }

            rowIndex++;
            currentRow = toRow + 1;
        }

        private static void WriteToeic(ExcelRange toeiceCell, MasterReportModel record, bool merge)
        {
            toeiceCell.Merge = merge;
            var qualifiedToeicBackground = Color.FromArgb(0, 176, 80);
            var failedToeicBackground = Color.FromArgb(255, 0, 0);

            if (record.Toeic.HasValue)
            {
                if (record.ToeicPolicy != 0)
                {
                    toeiceCell.Style.Font.Color.SetColor(record.ToeicPolicy > 0 ? qualifiedToeicBackground : failedToeicBackground);
                }

                toeiceCell.Value = record.Toeic;
            }
            else
            {
                toeiceCell.Value = "N/A";
            }
        }

        private static Color GetNominationStatusFontColor(NominationStatus statusId)
        {
            switch (statusId)
            {
                case NominationStatus.Shortlisted:
                    var shortlistedColor = Color.FromArgb(229, 97, 0);
                    return shortlistedColor;

                case NominationStatus.Winner:
                    var winnerColor = Color.FromArgb(20, 130, 213);
                    return winnerColor;

                default:
                    var nominatedColor = Color.FromArgb(79, 158, 0);
                    return nominatedColor;
            }
        }

        private static void WritePAFocus(ExcelWorksheet worksheet, int currentRow, int toRow, MasterReportModel record, bool merge)
        {
            var backgroundColor = Color.Empty;
            var textColor = Color.Empty;

            const int minPoint = 4;
            const int minHighlightedPoint = 8;

            var hasReadyPromotionSelect = bool.TryParse(record.IsReadyPromotion, out bool isReadyPromotion);

            if ((record.ContributionId + record.BonusGivenId) > minHighlightedPoint && hasReadyPromotionSelect)
            {
                backgroundColor = isReadyPromotion ? Color.FromArgb(89, 176, 59) : Color.FromArgb(104, 169, 189);
                textColor = Color.White;
            }
            else
            {
                if ((record.ContributionId + record.BonusGivenId) < minPoint)
                {
                    backgroundColor = Color.FromArgb(255, 95, 95);
                    textColor = Color.White;
                }
            }

            var contributionCell = worksheet.Cells[currentRow, 14, toRow, 14];
            contributionCell.Merge = merge;
            contributionCell.Value = record.Contribution;

            var bonusGivenCell = worksheet.Cells[currentRow, 15, toRow, 15];
            bonusGivenCell.Merge = merge;
            bonusGivenCell.Value = record.BonusGiven;

            var readyPromotionCell = worksheet.Cells[currentRow, 16, toRow, 16];
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

        private static float CalculateAverageGrade(IEnumerable<AssessmentModel> assessments, IEnumerable<PerformanceGrade> performanceGrades)
        {
            var points = assessments.Select(x =>
            {
                int.TryParse(x.GradeId, out int gradeId);
                var conversion = performanceGrades.FirstOrDefault(g => g.GradeId == gradeId);

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

                if (gradeId != unsatisfactoryGradeId || originalGradeId != 0)
                {
                    var icon = "";
                    var color = Color.Black;

                    if (gradeId > originalGradeId)
                    {
                        icon = "▲";
                        color = Color.Green;
                    }

                    if (gradeId < originalGradeId)
                    {
                        icon = "▼";
                        color = Color.Red;
                    }

                    var iconText = richTextCollection.Add(icon);
                    iconText.Bold = true;
                    iconText.Color = color;
                }
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