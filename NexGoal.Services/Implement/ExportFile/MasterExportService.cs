namespace NexGoal.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Core.Enumerations;
    using Core.Utilities;
    using NexGoal.Model.Report;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class MasterExportService : IMasterExportService
    {
        private const int GreenColor = 50;
        private const int SupperGreenColor = 51;
        private const int RedColor = 52;
        private const int EvenColor = 53;
        private const int HeaderColor = 54;
        private const int RowStartHeader = 3;
        private const int RowStartBody = 5;
        private const int TotalColumns = 19;

        private const int MinStone = 3;
        private const int MaxStone = 9;

        private HSSFWorkbook workbook;
        private ISheet sheet;
        private int columnWidth = 3000;
        private List<string> titleRow1Headers;

        public MemoryStream Export(IEnumerable<MasterReportModel> evaluations, string periodName)
        {
            this.workbook = new HSSFWorkbook();
            this.sheet = workbook.CreateSheet();
            SetColorIndex();

            RenderHearderPage(periodName);
            RenderNote();
            RenderHeaderTable();
            RenderBody(evaluations);

            this.sheet.CreateFreezePane(0, RowStartBody);

            return RenderToBytes();
        }

        private void RenderHearderPage(string periodName)
        {
            var reportTitle = string.Format("{0} - {1}", Lang.MasterReportTitleFile, periodName);

            IRow rowTitle = sheet.CreateRow(0);
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(reportTitle);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Left;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalColumns - 1));
        }

        private void RenderNote()
        {
            var note = "(*) " + Lang.SuitableForTeamGoals + ": " + Lang.PAFocusFirstNote
                + Environment.NewLine + "(**) " + Lang.DeserveRewards + ": " + Lang.PAFocusSecondNote;

            IRow row = sheet.CreateRow(2);
            row.Height = 700;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;

            ICell cell = row.CreateCell(0);
            cell.SetCellValue(note);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Left;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, TotalColumns - 1));
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderHeaderTable()
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HeaderColor;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HeaderColor;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(RowStartHeader);
            row.Height = 1000;
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
                Lang.SuitableForTeamGoals + " (*)",
                Lang.DeserveRewards + " (**)",
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
                cell.CellStyle.WrapText = true;
                cell.SetCellValue(titleRow1Headers[columnIndex]);
                sheet.AddMergedRegion(new CellRangeAddress(RowStartHeader, RowStartHeader + 1, columnIndex, columnIndex));
            }

            cell = row.CreateCell(columnIndex);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.AwardTitle);
            sheet.AddMergedRegion(new CellRangeAddress(RowStartHeader, RowStartHeader, columnIndex, columnIndex + 1));

            columnIndex += 2;
            cell = row.CreateCell(columnIndex);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.EvaluationByPM);
            sheet.AddMergedRegion(new CellRangeAddress(RowStartHeader, RowStartHeader, columnIndex, columnIndex + 2));

            columnIndex = titleRow1Headers.Count;

            row = sheet.CreateRow(RowStartHeader + 1);
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
            var rowIndex = RowStartBody;
            IFont redFont = workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;
            IFont whiteFont = workbook.CreateFont();
            whiteFont.Color = HSSFColor.White.Index;
            IFont greenFont = workbook.CreateFont();
            greenFont.Color = HSSFColor.Green.Index;

            var greenStyleLeft = workbook.CreateCellStyle();
            greenStyleLeft.Alignment = HorizontalAlignment.Left;
            greenStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            greenStyleLeft.FillForegroundColor = GreenColor;
            greenStyleLeft.FillPattern = FillPattern.SolidForeground;
            greenStyleLeft.FillBackgroundColor = GreenColor;
            greenStyleLeft.SetFont(whiteFont);
            SetBorderColor(greenStyleLeft);

            var greenStyleCenter = workbook.CreateCellStyle();
            greenStyleCenter.Alignment = HorizontalAlignment.Center;
            greenStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            greenStyleCenter.FillForegroundColor = GreenColor;
            greenStyleCenter.FillPattern = FillPattern.SolidForeground;
            greenStyleCenter.FillBackgroundColor = GreenColor;
            greenStyleCenter.SetFont(whiteFont);
            SetBorderColor(greenStyleCenter);

            var supperGreenStyleLeft = workbook.CreateCellStyle();
            supperGreenStyleLeft.Alignment = HorizontalAlignment.Left;
            supperGreenStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            supperGreenStyleLeft.FillForegroundColor = SupperGreenColor;
            supperGreenStyleLeft.FillPattern = FillPattern.SolidForeground;
            supperGreenStyleLeft.FillBackgroundColor = SupperGreenColor;
            supperGreenStyleLeft.SetFont(whiteFont);
            SetBorderColor(supperGreenStyleLeft);

            var supperGreenStyleCenter = workbook.CreateCellStyle();
            supperGreenStyleCenter.Alignment = HorizontalAlignment.Center;
            supperGreenStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            supperGreenStyleCenter.FillForegroundColor = SupperGreenColor;
            supperGreenStyleCenter.FillPattern = FillPattern.SolidForeground;
            supperGreenStyleCenter.FillBackgroundColor = SupperGreenColor;
            supperGreenStyleCenter.SetFont(whiteFont);
            SetBorderColor(supperGreenStyleCenter);

            var redStyleLeft = workbook.CreateCellStyle();
            redStyleLeft.Alignment = HorizontalAlignment.Left;
            redStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            redStyleLeft.FillForegroundColor = RedColor;
            redStyleLeft.FillPattern = FillPattern.SolidForeground;
            redStyleLeft.FillBackgroundColor = RedColor;
            redStyleLeft.SetFont(whiteFont);
            SetBorderColor(redStyleLeft);

            var redStyleCenter = workbook.CreateCellStyle();
            redStyleCenter.Alignment = HorizontalAlignment.Center;
            redStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            redStyleCenter.FillForegroundColor = RedColor;
            redStyleCenter.FillPattern = FillPattern.SolidForeground;
            redStyleCenter.FillBackgroundColor = RedColor;
            redStyleCenter.SetFont(whiteFont);
            SetBorderColor(redStyleCenter);

            var promotionEvenStyle = workbook.CreateCellStyle();
            promotionEvenStyle.Alignment = HorizontalAlignment.Center;
            promotionEvenStyle.VerticalAlignment = VerticalAlignment.Center;
            promotionEvenStyle.FillForegroundColor = EvenColor;
            promotionEvenStyle.FillPattern = FillPattern.SolidForeground;
            promotionEvenStyle.FillBackgroundColor = EvenColor;
            promotionEvenStyle.SetFont(greenFont);
            SetBorderColor(promotionEvenStyle);

            var promotionOddStyle = workbook.CreateCellStyle();
            promotionOddStyle.Alignment = HorizontalAlignment.Center;
            promotionOddStyle.VerticalAlignment = VerticalAlignment.Center;
            promotionOddStyle.SetFont(greenFont);
            SetBorderColor(promotionOddStyle);

            var poorEvenStyle = workbook.CreateCellStyle();
            poorEvenStyle.Alignment = HorizontalAlignment.Center;
            poorEvenStyle.VerticalAlignment = VerticalAlignment.Center;
            poorEvenStyle.FillForegroundColor = EvenColor;
            poorEvenStyle.FillPattern = FillPattern.SolidForeground;
            poorEvenStyle.FillBackgroundColor = EvenColor;
            poorEvenStyle.SetFont(redFont);
            SetBorderColor(poorEvenStyle);

            var poorOddStyle = workbook.CreateCellStyle();
            poorOddStyle.Alignment = HorizontalAlignment.Center;
            poorOddStyle.VerticalAlignment = VerticalAlignment.Center;
            poorOddStyle.SetFont(redFont);
            SetBorderColor(poorOddStyle);

            var evenStyleLeft = workbook.CreateCellStyle();
            evenStyleLeft.Alignment = HorizontalAlignment.Left;
            evenStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            evenStyleLeft.FillForegroundColor = EvenColor;
            evenStyleLeft.FillPattern = FillPattern.SolidForeground;
            evenStyleLeft.FillBackgroundColor = EvenColor;
            SetBorderColor(evenStyleLeft);

            var evenStyleCenter = workbook.CreateCellStyle();
            evenStyleCenter.Alignment = HorizontalAlignment.Center;
            evenStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            evenStyleCenter.FillForegroundColor = EvenColor;
            evenStyleCenter.FillPattern = FillPattern.SolidForeground;
            evenStyleCenter.FillBackgroundColor = EvenColor;
            SetBorderColor(evenStyleCenter);

            ICellStyle oddStyleLeft = workbook.CreateCellStyle();
            oddStyleLeft.Alignment = HorizontalAlignment.Left;
            oddStyleLeft.VerticalAlignment = VerticalAlignment.Center;
            SetBorderColor(oddStyleLeft);

            ICellStyle oddStyleCenter = workbook.CreateCellStyle();
            oddStyleCenter.Alignment = HorizontalAlignment.Center;
            oddStyleCenter.VerticalAlignment = VerticalAlignment.Center;
            SetBorderColor(oddStyleCenter);

            ICell cellBody;
            int index = 1;
            foreach (var employee in employeeEvaluations)
            {
                IRow row = sheet.CreateRow(rowIndex);
                ICellStyle bodyStyleCenter = workbook.CreateCellStyle();
                ICellStyle bodyStyleLeft = workbook.CreateCellStyle();

                var columnIndex = 0;
                var rowIndexBeginMerger = rowIndex;
                var condition = employee.PAFocus.BonusGiven + employee.PAFocus.Contribution;

                if (condition >= MaxStone)
                {
                    if (employee.PAFocus.IsReadyPromotion.HasValue && employee.PAFocus.IsReadyPromotion.Value)
                    {
                        bodyStyleCenter = supperGreenStyleCenter;
                        bodyStyleLeft = supperGreenStyleLeft;
                    }
                    else
                    {
                        bodyStyleCenter = greenStyleCenter;
                        bodyStyleLeft = greenStyleLeft;
                    }
                }
                else if (condition <= MinStone)
                {
                    bodyStyleCenter = redStyleCenter;
                    bodyStyleLeft = redStyleLeft;
                }
                else
                {
                    if (index % 2 == 0)
                    {
                        bodyStyleCenter = evenStyleCenter;
                        bodyStyleLeft = evenStyleLeft;
                    }
                    else
                    {
                        bodyStyleCenter = oddStyleCenter;
                        bodyStyleLeft = oddStyleLeft;
                    }
                }

                cellBody = row.CreateCell(columnIndex++);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(index);

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
                if (promotion == Lang.Yes && condition > MinStone && condition < MaxStone)
                {
                    if (index % 2 == 0)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = promotionEvenStyle;
                        cellBody.SetCellValue(promotion);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = promotionOddStyle;
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
                if (performance == Lang.Yes && condition > MinStone && condition < MaxStone)
                {
                    if (index % 2 == 0)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = poorEvenStyle;
                        cellBody.SetCellValue(performance);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = poorOddStyle;
                        cellBody.SetCellValue(promotion);
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
                sheet.SetColumnWidth(columnIndex++, columnWidth * 10);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = string.IsNullOrEmpty(employee.PAFocus.NextDescriptionObjective) ? bodyStyleCenter : bodyStyleLeft;
                cellBody.SetCellValue(string.IsNullOrEmpty(employee.PAFocus.NextDescriptionObjective) ? Lang.Dash : employee.PAFocus.NextDescriptionObjective);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 10);

                var maxRowOfEmployee = GetNumberRowOfEmployee(employee);
                for (int i = 0; i < maxRowOfEmployee; i++)
                {
                    if (i > 0)
                    {
                        row = sheet.CreateRow(rowIndex);
                    }

                    if (i < employee.Nominations.Count && i < employee.EmployeeWorkings.Count)
                    {
                        var j = columnIndex;

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

                    if (i >= employee.Nominations.Count && i < employee.EmployeeWorkings.Count)
                    {
                        var j = columnIndex;

                        cellBody = row.CreateCell(j);
                        cellBody.CellStyle = bodyStyleCenter;
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

                    if (i < employee.Nominations.Count && i >= employee.EmployeeWorkings.Count)
                    {
                        var j = columnIndex;

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
                        cellBody.SetCellValue(string.Empty);
                        sheet.SetColumnWidth(j++, columnWidth * 2);

                        cellBody = row.CreateCell(j);
                        cellBody.CellStyle = bodyStyleCenter;
                        cellBody.SetCellValue(string.Empty);
                        sheet.SetColumnWidth(j++, columnWidth * 2);

                        cellBody = row.CreateCell(j);
                        cellBody.CellStyle = bodyStyleLeft;
                        cellBody.SetCellValue(string.Empty);
                        sheet.SetColumnWidth(j++, columnWidth * 4);
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

                index++;
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

        private void SetColorIndex()
        {
            HSSFPalette palette = workbook.GetCustomPalette();
            palette.SetColorAtIndex(SupperGreenColor, (byte)89, (byte)176, (byte)59);
            palette.SetColorAtIndex(GreenColor, (byte)104, (byte)169, (byte)189);
            palette.SetColorAtIndex(RedColor, (byte)255, (byte)95, (byte)95);
            palette.SetColorAtIndex(EvenColor, (byte)249, (byte)249, (byte)249);
            palette.SetColorAtIndex(HeaderColor, (byte)50, (byte)67, (byte)115);
        }

        private void SetBorderColor(ICellStyle cell)
        {
            cell.TopBorderColor = HSSFColor.Grey40Percent.Index;
            cell.RightBorderColor = HSSFColor.Grey40Percent.Index;
            cell.BottomBorderColor = HSSFColor.Grey40Percent.Index;
            cell.RightBorderColor = HSSFColor.Grey40Percent.Index;
        }
    }
}