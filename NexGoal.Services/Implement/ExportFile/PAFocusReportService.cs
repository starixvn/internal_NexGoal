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

            ICellStyle bodyStyleLeft = workbook.CreateCellStyle();
            ICellStyle bodyStyleCenter = workbook.CreateCellStyle();

            ICellStyle bodyStyleLeft1 = workbook.CreateCellStyle();
            bodyStyleLeft1.Alignment = HorizontalAlignment.Left;
            bodyStyleLeft1.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyleLeft2 = workbook.CreateCellStyle();
            bodyStyleLeft2.Alignment = HorizontalAlignment.Left;
            bodyStyleLeft2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyleLeft2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyleLeft2.FillPattern = FillPattern.SolidForeground;
            bodyStyleLeft2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            ICellStyle bodyStyleCenter1 = workbook.CreateCellStyle();
            bodyStyleCenter1.Alignment = HorizontalAlignment.Center;
            bodyStyleCenter1.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyleRedCenter1 = workbook.CreateCellStyle();
            bodyStyleRedCenter1.Alignment = HorizontalAlignment.Center;
            bodyStyleRedCenter1.VerticalAlignment = VerticalAlignment.Center;
            bodyStyleRedCenter1.SetFont(redFont);

            ICellStyle bodyStyleBlueCenter1 = workbook.CreateCellStyle();
            bodyStyleBlueCenter1.Alignment = HorizontalAlignment.Center;
            bodyStyleBlueCenter1.VerticalAlignment = VerticalAlignment.Center;
            bodyStyleBlueCenter1.SetFont(blueFont);

            ICellStyle bodyStyleCenter2 = workbook.CreateCellStyle();
            bodyStyleCenter2.Alignment = HorizontalAlignment.Center;
            bodyStyleCenter2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyleCenter2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyleCenter2.FillPattern = FillPattern.SolidForeground;
            bodyStyleCenter2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            ICellStyle bodyStyleBlueCenter2 = workbook.CreateCellStyle();
            bodyStyleBlueCenter2.Alignment = HorizontalAlignment.Center;
            bodyStyleBlueCenter2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyleBlueCenter2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyleBlueCenter2.FillPattern = FillPattern.SolidForeground;
            bodyStyleBlueCenter2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyleBlueCenter2.SetFont(blueFont);

            ICellStyle bodyStyleRedCenter2 = workbook.CreateCellStyle();
            bodyStyleRedCenter2.Alignment = HorizontalAlignment.Center;
            bodyStyleRedCenter2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyleRedCenter2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyleRedCenter2.FillPattern = FillPattern.SolidForeground;
            bodyStyleRedCenter2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyleRedCenter2.SetFont(redFont);

            ICell cellBody;

            foreach (PAFocusModel pa in focuses)
            {
                columnIndex = 0;
                if (rowIndex % 2 == 0)
                {
                    bodyStyleLeft = bodyStyleLeft2;
                    bodyStyleCenter = bodyStyleCenter2;
                }
                else
                {
                    bodyStyleLeft = bodyStyleLeft1;
                    bodyStyleCenter = bodyStyleCenter1;
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
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(pa.Employee.Group.GroupName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var bonusGiven = pa.BonusGiven.HasValue ? Enumerations.GetDescription((PAFocusType)pa.BonusGiven.Value) : string.Empty;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(bonusGiven);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var contribution = pa.Contribution.HasValue ? Enumerations.GetDescription((PAFocusType)pa.Contribution.Value) : string.Empty;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(contribution);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var promotion = pa.IsReadyPromotion.HasValue ? pa.IsReadyPromotion.Value ? Lang.Yes : Lang.No : string.Empty;
                if (promotion == Lang.Yes)
                {
                    if (rowIndex % 2 == 0)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = bodyStyleBlueCenter2;
                        cellBody.SetCellValue(promotion);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = bodyStyleBlueCenter1;
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

                var performance = pa.IsPoorPerformance.HasValue ? pa.IsPoorPerformance.Value ? Lang.Yes : Lang.No : string.Empty;
                if (performance == Lang.Yes)
                {
                    if (rowIndex % 2 == 0)
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = bodyStyleRedCenter2;
                        cellBody.SetCellValue(performance);
                        sheet.SetColumnWidth(columnIndex++, columnWidth * 2);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex);
                        cellBody.CellStyle = bodyStyleRedCenter1;
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
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(pa.LastCompletedPercentObjective);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                var score = pa.FinalScore.HasValue && pa.FinalScore.Value != 0 ? pa.FinalScore.ToString() : Lang.NotAvailable;
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
