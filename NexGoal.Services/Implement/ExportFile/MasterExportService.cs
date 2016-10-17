namespace NexGoal.Services.Implement
{
    using Core.Enumerations;
    using Core.Utilities;
    using NexGoal.Model.Report;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public class MasterExportService : IMasterExportService
    {
        private HSSFWorkbook workbook;
        private ISheet sheet;
        private int rowIndex = 0;
        private int columnWidth = 2560;
        private List<string> titleRow1Headers;

        public MemoryStream Export(IEnumerable<MasterReportModel> evaluations, string periodName)
        {
            this.workbook = new HSSFWorkbook();
            this.sheet = workbook.CreateSheet();

            RenderHearderPage(periodName);
            RenderHeaderTable();
            RenderBody(evaluations);

            return RenderToBytes();
        }

        private void RenderHearderPage(string periodName)
        {
            var reportTitle = string.Format("{0} - {1}", Lang.MasterReportTitleFile, periodName);

            IRow rowTitle = sheet.CreateRow(rowIndex);
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(reportTitle);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 12));

            rowIndex += 2;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderHeaderTable()
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(rowIndex);
            titleRow1Headers = new List<string>
            {
                Lang.SymbolCount,
                Lang.NickName,
                Lang.FullName,
                Lang.JobTitle,
                Lang.Group,
                Lang.ReviewerAdjustment,
                Lang.PAFinalScore,
                Lang.EvaluatorComment,
                Lang.SuitableForTeamGoals,
                Lang.DeserveRewards,
                Lang.ApplicableToPromote,
                Lang.PoorPerformance,
                Lang.AccomplishedCurrentObjectives,
                Lang.NextObjectives
            };
            ICell cell;
            int columnIndex = 0;
            for (columnIndex = 0; columnIndex < titleRow1Headers.Count; columnIndex++)
            {
                cell = row.CreateCell(columnIndex);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(titleRow1Headers[columnIndex]);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + 1, columnIndex, columnIndex));
            }

            cell = row.CreateCell(columnIndex);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.AwardTitle);
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, columnIndex, columnIndex + 1));

            columnIndex += 2;
            cell = row.CreateCell(columnIndex);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.EvaluationByPM);
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, columnIndex, columnIndex + 2));

            columnIndex = titleRow1Headers.Count;
            rowIndex++;

            row = sheet.CreateRow(rowIndex);
            cell = row.CreateCell(columnIndex++);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.Title);

            cell = row.CreateCell(columnIndex++);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.Status);

            cell = row.CreateCell(columnIndex++);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.ProjectName);

            cell = row.CreateCell(columnIndex++);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.TotalScore);

            cell = row.CreateCell(columnIndex++);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.Comment);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderBody(IEnumerable<MasterReportModel> employeeEvaluations)
        {
            rowIndex++;
            IFont redFont = workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

            IFont blueFont = workbook.CreateFont();
            blueFont.Color = HSSFColor.Green.Index;

            ICellStyle bodyStyleCenter = workbook.CreateCellStyle();
            ICellStyle bodyStyleLeft = workbook.CreateCellStyle();

            ICellStyle goldStyleLeft = workbook.CreateCellStyle();
            goldStyleLeft.Alignment = HorizontalAlignment.Left;
            goldStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            goldStyleLeft.FillForegroundColor = HSSFColor.LightYellow.Index;
            goldStyleLeft.FillPattern = FillPattern.SolidForeground;
            goldStyleLeft.FillBackgroundColor = HSSFColor.LightYellow.Index;

            ICellStyle goldStyleCenter = workbook.CreateCellStyle();
            goldStyleCenter.Alignment = HorizontalAlignment.Center;
            goldStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            goldStyleCenter.FillForegroundColor = HSSFColor.LightYellow.Index;
            goldStyleCenter.FillPattern = FillPattern.SolidForeground;
            goldStyleCenter.FillBackgroundColor = HSSFColor.LightYellow.Index;

            ICellStyle goldBlueStyleCenter = workbook.CreateCellStyle();
            goldBlueStyleCenter.Alignment = HorizontalAlignment.Center;
            goldBlueStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            goldBlueStyleCenter.FillForegroundColor = HSSFColor.LightYellow.Index;
            goldBlueStyleCenter.FillPattern = FillPattern.SolidForeground;
            goldBlueStyleCenter.FillBackgroundColor = HSSFColor.LightYellow.Index;
            goldBlueStyleCenter.SetFont(blueFont);

            ICellStyle goldRedStyleCenter = workbook.CreateCellStyle();
            goldRedStyleCenter.Alignment = HorizontalAlignment.Center;
            goldRedStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            goldRedStyleCenter.FillForegroundColor = HSSFColor.LightYellow.Index;
            goldRedStyleCenter.FillPattern = FillPattern.SolidForeground;
            goldRedStyleCenter.FillBackgroundColor = HSSFColor.LightYellow.Index;
            goldRedStyleCenter.SetFont(redFont);

            ICellStyle normalStyleLeft = workbook.CreateCellStyle();
            normalStyleLeft.Alignment = HorizontalAlignment.Left;
            normalStyleLeft.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle normalStyleCenter = workbook.CreateCellStyle();
            normalStyleCenter.Alignment = HorizontalAlignment.Center;
            normalStyleCenter.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle normalBlueStyleCenter = workbook.CreateCellStyle();
            normalBlueStyleCenter.Alignment = HorizontalAlignment.Center;
            normalBlueStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            normalBlueStyleCenter.SetFont(blueFont);

            ICellStyle normalRedStyleCenter = workbook.CreateCellStyle();
            normalRedStyleCenter.Alignment = HorizontalAlignment.Center;
            normalRedStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            normalRedStyleCenter.SetFont(redFont);

            ICellStyle roseStyleCenter = workbook.CreateCellStyle();
            roseStyleCenter.Alignment = HorizontalAlignment.Center;
            roseStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            roseStyleCenter.FillForegroundColor = HSSFColor.Rose.Index;
            roseStyleCenter.FillPattern = FillPattern.SolidForeground;
            roseStyleCenter.FillBackgroundColor = HSSFColor.Rose.Index;

            ICellStyle roseBlueStyleCenter = workbook.CreateCellStyle();
            roseBlueStyleCenter.Alignment = HorizontalAlignment.Center;
            roseBlueStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            roseBlueStyleCenter.FillForegroundColor = HSSFColor.Rose.Index;
            roseBlueStyleCenter.FillPattern = FillPattern.SolidForeground;
            roseBlueStyleCenter.FillBackgroundColor = HSSFColor.Rose.Index;
            roseBlueStyleCenter.SetFont(blueFont);

            ICellStyle roseRedStyleCenter = workbook.CreateCellStyle();
            roseRedStyleCenter.Alignment = HorizontalAlignment.Center;
            roseRedStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            roseRedStyleCenter.FillForegroundColor = HSSFColor.Rose.Index;
            roseRedStyleCenter.FillPattern = FillPattern.SolidForeground;
            roseRedStyleCenter.FillBackgroundColor = HSSFColor.Rose.Index;
            roseRedStyleCenter.SetFont(redFont);

            ICellStyle roseStyleLeft = workbook.CreateCellStyle();
            roseStyleLeft.Alignment = HorizontalAlignment.Left;
            roseStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            roseStyleLeft.FillForegroundColor = HSSFColor.Rose.Index;
            roseStyleLeft.FillPattern = FillPattern.SolidForeground;
            roseStyleLeft.FillBackgroundColor = HSSFColor.Rose.Index;

            ICell cellBody;
            int index = 1;
            foreach (var employee in employeeEvaluations)
            {
                var columnIndex = 0;
                var rowIndexBeginMerger = rowIndex;
                if (employee.PAFocus.BonusGiven + employee.PAFocus.Contribution == 10)
                {
                    bodyStyleCenter = goldStyleCenter;
                    bodyStyleLeft = goldStyleLeft;
                }
                else if (employee.PAFocus.BonusGiven + employee.PAFocus.Contribution == 9)
                {
                    bodyStyleCenter = roseStyleCenter;
                    bodyStyleLeft = roseStyleLeft;
                }
                else
                {
                    bodyStyleCenter = normalStyleCenter;
                    bodyStyleLeft = normalStyleLeft;
                }

                IRow row = sheet.CreateRow(rowIndex);

                cellBody = row.CreateCell(columnIndex++);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(index++);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(employee.PAFocus.Employee.NickName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(employee.PAFocus.Employee.FullName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(employee.PAFocus.Employee.JobTitle.JobTitleName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(employee.PAFocus.Employee.Group.GroupName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var adjustmentScore = FormatReviewerAjustment(employee.ReviewerAdjustment);
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(adjustmentScore);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var score = employee.PAFocus.FinalScore.HasValue && employee.PAFocus.FinalScore.Value != 0 ? employee.PAFocus.FinalScore.ToString() : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(score);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var comment = FormatEvaluatorComment(employee.EvaluatorComments);
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = string.IsNullOrEmpty(comment) ? bodyStyleCenter : bodyStyleLeft;
                cellBody.SetCellValue(string.IsNullOrEmpty(comment) ? Lang.Dash : comment);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 4);

                var contribution = employee.PAFocus.Contribution.HasValue ? Enumerations.GetDescription((PAFocusType)employee.PAFocus.Contribution.Value) : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(contribution);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var bonusGiven = employee.PAFocus.BonusGiven.HasValue ? Enumerations.GetDescription((PAFocusType)employee.PAFocus.BonusGiven.Value) : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(bonusGiven);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var promotion = employee.PAFocus.IsReadyPromotion.HasValue ? employee.PAFocus.IsReadyPromotion.Value ? Lang.Yes : Lang.No : Lang.Dash;
                if (promotion == Lang.Yes)
                {
                    if (employee.PAFocus.BonusGiven + employee.PAFocus.Contribution == 10)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = goldBlueStyleCenter;
                        cellBody.SetCellValue(promotion);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else if (employee.PAFocus.BonusGiven + employee.PAFocus.Contribution == 9)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = roseBlueStyleCenter;
                        cellBody.SetCellValue(promotion);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = normalBlueStyleCenter;
                        cellBody.SetCellValue(promotion);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                }
                else
                {
                    cellBody = row.CreateCell(columnIndex);
                    cellBody.CellStyle = bodyStyleCenter;
                    cellBody.SetCellValue(promotion);
                    sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                }

                var performance = employee.PAFocus.IsPoorPerformance.HasValue ? employee.PAFocus.IsPoorPerformance.Value ? Lang.Yes : Lang.No : Lang.Dash;
                if (performance == Lang.Yes)
                {
                    if (employee.PAFocus.BonusGiven + employee.PAFocus.Contribution == 10)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = goldRedStyleCenter;
                        cellBody.SetCellValue(performance);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else if (employee.PAFocus.BonusGiven + employee.PAFocus.Contribution == 9)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = roseRedStyleCenter;
                        cellBody.SetCellValue(performance);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = normalRedStyleCenter;
                        cellBody.SetCellValue(performance);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                }
                else
                {
                    cellBody = row.CreateCell(columnIndex);
                    cellBody.CellStyle = bodyStyleCenter;
                    cellBody.SetCellValue(performance);
                    sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                }

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = string.IsNullOrEmpty(employee.PAFocus.LastCompletedPercentObjective) ? bodyStyleCenter : bodyStyleLeft;
                cellBody.SetCellValue(string.IsNullOrEmpty(employee.PAFocus.LastCompletedPercentObjective) ? Lang.Dash : employee.PAFocus.LastCompletedPercentObjective);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 4);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = string.IsNullOrEmpty(employee.PAFocus.NextDescriptionObjective) ? bodyStyleCenter : bodyStyleLeft;
                cellBody.SetCellValue(string.IsNullOrEmpty(employee.PAFocus.NextDescriptionObjective) ? Lang.Dash : employee.PAFocus.NextDescriptionObjective);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 4);

                var maxRowOfEmployee = GetNumberRowOfEmployee(employee);
                for (int i = 0; i < maxRowOfEmployee; i++)
                {
                    if (i < employee.Nominations.Count)
                    {
                        var j = columnIndex;
                        if (i > 0)
                        {
                            row = sheet.CreateRow(rowIndex);
                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.Nominations[i].Title);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(employee.Nominations[i].StatusToString);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].ProjectName);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].DecryptedScore);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].Comment);
                            sheet.SetColumnWidth(j++, columnWidth * 4);
                        }
                        else
                        {
                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.Nominations[i].Title);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(employee.Nominations[i].StatusToString);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].ProjectName);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].DecryptedScore);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].Comment);
                            sheet.SetColumnWidth(j++, columnWidth * 4);
                        }
                    }

                    if (i >= employee.Nominations.Count && i < employee.EmployeeWorkings.Count)
                    {
                        var j = columnIndex;
                        if (i > 0)
                        {
                            row = sheet.CreateRow(rowIndex);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(string.Empty);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(string.Empty);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].ProjectName);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].DecryptedScore);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].Comment);
                            sheet.SetColumnWidth(j++, columnWidth * 4);
                        }
                        else
                        {
                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(string.Empty);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(string.Empty);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].ProjectName);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleCenter;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].DecryptedScore);
                            sheet.SetColumnWidth(j++, columnWidth * 2);

                            cellBody = row.CreateCell(j);
                            cellBody.CellStyle = bodyStyleLeft;
                            cellBody.SetCellValue(employee.EmployeeWorkings[i].Comment);
                            sheet.SetColumnWidth(j++, columnWidth * 4);
                        }
                    }

                    rowIndex++;
                }

                for (int m = 0; m < titleRow1Headers.Count; m++)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndexBeginMerger, rowIndexBeginMerger + maxRowOfEmployee - 1, m, m));
                }

                if (maxRowOfEmployee == 0)
                {
                    rowIndex++;
                }
            }
        }

        private string FormatEvaluatorComment(IEnumerable<EvaluatorCommentReportModel> comments)
        {
            var result = string.Empty;
            foreach (var evaluator in comments)
            {
                var comment = string.Format("{0}: {1} ({2}) \n {3} \n \n", Lang.Evaluator, evaluator.EvaluatorFullName, evaluator.EvaluatorNickName, evaluator.Comment);
                result += comment;
            }

            return result;
        }

        private int GetNumberRowOfEmployee(MasterReportModel evaluation)
        {
            int count = evaluation.Nominations.Count;
            if (evaluation.EmployeeWorkings.Count > count)
            {
                count = evaluation.EmployeeWorkings.Count;
            }

            return count > 1 ? count : 1;
        }

        private string FormatReviewerAjustment(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Lang.Dash;
            }

            var adjustmentScore = int.Parse(value);
            if (adjustmentScore > 0)
            {
                return "+" + adjustmentScore.ToString();
            }

            return adjustmentScore.ToString();
        }

        private MemoryStream RenderToBytes()
        {
            using (var output = new MemoryStream())
            {
                this.workbook.Write(output);

                return output;
            }
        }
    }
}