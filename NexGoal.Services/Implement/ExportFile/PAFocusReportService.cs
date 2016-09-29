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
using System.Linq;

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
                Lang.Team,
                Lang.No,
                Lang.NickName,
                Lang.FullName,
                Lang.EPAScore,
                Lang.PersonalObjectives,
                Lang.Promotability,
                Lang.Contribution,
                string.Format("{0} ({1})", Lang.BonusGiven, Lang.Points),
                string.Empty,
                Lang.BonusRanking,
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

            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 8, 9));
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderBodyTable(IEnumerable<PAFocusModel> focuses)
        {
            var rowIndex = 3;
            var columnIndex = 0;
            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.Left;
            bodyStyle1.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.Left;
            bodyStyle2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyle2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle2.FillPattern = FillPattern.SolidForeground;
            bodyStyle2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            IFont font = workbook.CreateFont();
            font.Color = HSSFColor.White.Index;

            ICellStyle redStyle = workbook.CreateCellStyle();
            redStyle.Alignment = HorizontalAlignment.Center;
            redStyle.VerticalAlignment = VerticalAlignment.Center;
            redStyle.FillForegroundColor = HSSFColor.Red.Index;
            redStyle.FillPattern = FillPattern.SolidForeground;
            redStyle.FillBackgroundColor = HSSFColor.Red.Index;
            redStyle.SetFont(font);

            ICellStyle blueStyle = workbook.CreateCellStyle();
            blueStyle.Alignment = HorizontalAlignment.Center;
            blueStyle.VerticalAlignment = VerticalAlignment.Center;
            blueStyle.FillForegroundColor = HSSFColor.Green.Index;
            blueStyle.FillPattern = FillPattern.SolidForeground;
            blueStyle.FillBackgroundColor = HSSFColor.Green.Index;
            blueStyle.SetFont(font);

            ICell cellBody;
            var groupNames = GroupPAFocus(focuses);
            foreach (var group in groupNames)
            {
                var index = 1;
                var firstRow = rowIndex;
                float total = 0;
                foreach (PAFocusModel pa in focuses.Where(i => i.Employee.Group.GroupName == group))
                {
                    if (rowIndex % 2 == 0)
                    {
                        bodyStyle = bodyStyle2;
                    }
                    else
                    {
                        bodyStyle = bodyStyle1;
                    }

                    columnIndex = 1;

                    IRow row = sheet.CreateRow(rowIndex);
                    cellBody = row.CreateCell(0);
                    cellBody.CellStyle = bodyStyle1;
                    cellBody.SetCellValue(group);
                    sheet.SetColumnWidth(0, columnWidth * 2);

                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(index++);

                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(pa.Employee.NickName);
                    sheet.SetColumnWidth(2, columnWidth);

                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(pa.Employee.FullName);
                    sheet.SetColumnWidth(3, columnWidth * 2);

                    var score = pa.FinalScore.HasValue ? pa.FinalScore.ToString() + "/100" : Lang.NotAvailable;
                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(score);
                    sheet.SetColumnWidth(4, columnWidth);

                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(pa.LastCompletedPercentObjective);
                    sheet.SetColumnWidth(5, columnWidth);

                    var promotion = pa.IsReadyPromotion.HasValue ? pa.IsReadyPromotion.ToString() : string.Empty;
                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(promotion);
                    sheet.SetColumnWidth(6, columnWidth);

                    var contribution = pa.Contribution.HasValue ? pa.Contribution.ToString() : string.Empty;
                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(contribution);
                    sheet.SetColumnWidth(7, columnWidth);

                    var bonusGiven = pa.BonusGiven.HasValue ? pa.BonusGiven.ToString() : string.Empty;
                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(bonusGiven);
                    sheet.SetColumnWidth(7, columnWidth);

                    total += pa.BonusGiven.HasValue ? pa.BonusGiven.Value : 0;
                    if (total < 100)
                    {
                        cellBody = row.CreateCell(columnIndex++);
                        cellBody.CellStyle = redStyle;
                        cellBody.SetCellValue(total + "/100");
                        sheet.SetColumnWidth(8, columnWidth);
                    }
                    else
                    {
                        cellBody = row.CreateCell(columnIndex++);
                        cellBody.CellStyle = blueStyle;
                        cellBody.SetCellValue(total + "/100");
                        sheet.SetColumnWidth(8, columnWidth);
                    }

                    cellBody = row.CreateCell(columnIndex++);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(pa.NextDescriptionObjective);
                    sheet.SetColumnWidth(11, columnWidth * 10);

                    rowIndex++;
                }

                sheet.AddMergedRegion(new CellRangeAddress(firstRow, rowIndex - 1, 0, 0));
                sheet.AddMergedRegion(new CellRangeAddress(firstRow, rowIndex - 1, 9, 9));
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

        private List<string> GroupPAFocus(IEnumerable<PAFocusModel> focuses)
        {
            var groupNames = new List<string>();
            foreach (var focus in focuses)
            {
                if (!groupNames.Contains(focus.Employee.Group.GroupName))
                {
                    groupNames.Add(focus.Employee.Group.GroupName);
                }
            }

            return groupNames;
        }
    }
}
