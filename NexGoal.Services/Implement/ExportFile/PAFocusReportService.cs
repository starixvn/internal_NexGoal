using NexGoal.Core.Enumerations;
using NexGoal.Core.Utilities;
using NexGoal.Model.PAFocus;
using NexGoal.Resources;
using NexGoal.Services.Interfaces;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NexGoal.Services.Implement
{
    public class PAFocusReportService : IPAFocusReportService
    {
        private readonly IPeriodService periodService;
        private IList<string> titleHeaders;
        private HSSFWorkbook workbook;
        private ISheet sheet;
        private int columnWidth = 2560;

        public PAFocusReportService()
            : this(null)
        {
        }

        public PAFocusReportService(IPeriodService periodService)
        {
            this.periodService = periodService ?? new PeriodService();
        }

        public MemoryStream ExportPAFocus(IEnumerable<PAFocusModel> focuses, int periodId)
        {
            this.titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.FullName,
                Lang.NickName,
                Lang.JobTitle,
                Lang.Team,
                Lang.SuitableForTeamGoals,
                Lang.DeserveRewards,
                Lang.ApplicableToPromote,
                Lang.PoorPerformance,
                Lang.AccomplishedCurrentObjectives,
                Lang.PAScore,
                Lang.NextObjectives
            };
            this.workbook = new HSSFWorkbook();
            this.sheet = workbook.CreateSheet();

            RenderHeaderPage(periodId);
            RenderHeaderTable();
            RenderBodyTable(focuses);

            return RenderToBytes();
        }

        private void RenderHeaderPage(int periodId)
        {
            var period = periodService.GetPeriodById(periodId);
            var title = "PA Focus of " + period.Name;
            IRow rowTitle = sheet.CreateRow(0);
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(title);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));
        }

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

            IRow row = sheet.CreateRow(2);
            for (int i = 0; i < titleHeaders.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(titleHeaders[i]);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderBodyTable(IEnumerable<PAFocusModel> focuses)
        {
            var rowIndex = 3;
            var columnIndex = 0;
            var index = 1;

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

            foreach (PAFocusModel pa in focuses)
            {
                columnIndex = 0;
                if (pa.BonusGiven + pa.Contribution == 10)
                {
                    bodyStyleCenter = goldStyleCenter;
                    bodyStyleLeft = goldStyleLeft;
                }
                else if (pa.BonusGiven + pa.Contribution == 9)
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
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(pa.Employee.FullName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(pa.Employee.NickName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(pa.Employee.JobTitle.JobTitleName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(pa.Employee.Group.GroupName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var bonusGiven = pa.BonusGiven.HasValue ? Enumerations.GetDescription((PAFocusType)pa.BonusGiven.Value) : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(bonusGiven);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var contribution = pa.Contribution.HasValue ? Enumerations.GetDescription((PAFocusType)pa.Contribution.Value) : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(contribution);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var promotion = pa.IsReadyPromotion.HasValue ? pa.IsReadyPromotion.Value ? Lang.Yes : Lang.No : Lang.Dash;
                if (promotion == Lang.Yes)
                {
                    if (pa.BonusGiven + pa.Contribution == 10)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = goldBlueStyleCenter;
                        cellBody.SetCellValue(promotion);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else if (pa.BonusGiven + pa.Contribution == 9)
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

                var performance = pa.IsPoorPerformance.HasValue ? pa.IsPoorPerformance.Value ? Lang.Yes : Lang.No : Lang.Dash;
                if (performance == Lang.Yes)
                {
                    if (pa.BonusGiven + pa.Contribution == 10)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = goldRedStyleCenter;
                        cellBody.SetCellValue(performance);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else if (pa.BonusGiven + pa.Contribution == 9)
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
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(pa.LastCompletedPercentObjective);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var score = pa.FinalScore.HasValue && pa.FinalScore.Value != 0 ? pa.FinalScore.ToString() : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(score);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(pa.NextDescriptionObjective);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 10);

                rowIndex++;
            }
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
