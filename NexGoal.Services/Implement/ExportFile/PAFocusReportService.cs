namespace NexGoal.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;
    using NexGoal.Model.PAFocus;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class PAFocusReportService : IPAFocusReportService
    {
        private const int GreenColor = 50;
        private const int SupperGreenColor = 51;
        private const int RedColor = 52;
        private const int EvenColor = 53;
        private const int HeaderColor = 54;
        private const int RowStartHeader = 3;
        private const int RowStartBody = 4;
        private const int TotalColumns = 12;

        private const int MinStone = 3;
        private const int MaxStone = 9;

        private readonly IPeriodService periodService;
        private IList<string> titleHeaders;
        private HSSFWorkbook workbook;
        private HSSFSheet sheet;
        private int columnWidth = 3000;

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
                Lang.NickName,
                Lang.FullName,
                Lang.JobTitle,
                Lang.Team,
                Lang.SuitableForTeamGoals + " (*)",
                Lang.DeserveRewards + " (**)",
                Lang.ApplicableToPromote,
                Lang.PoorPerformance,
                Lang.AccomplishedCurrentObjectives,
                Lang.PAScore,
                Lang.NextObjectives
            };
            this.workbook = new HSSFWorkbook();

            this.sheet = workbook.CreateSheet() as HSSFSheet;

            SetColorIndex();
            RenderHeaderPage(periodId);
            RenderNote();
            RenderHeaderTable();
            RenderBodyTable(focuses);

            this.sheet.CreateFreezePane(0, RowStartBody);

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
            cellStyle.WrapText = true;
            SetBorderColor(cellStyle);
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(RowStartHeader);
            row.Height = 1000;
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
            var rowIndex = RowStartBody;
            var columnIndex = 0;
            var index = 1;

            IFont whiteFont = workbook.CreateFont();
            whiteFont.Color = HSSFColor.White.Index;
            IFont greenFont = workbook.CreateFont();
            greenFont.Color = HSSFColor.Green.Index;
            IFont redFont = workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

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
            supperGreenStyleLeft.FillBackgroundColor = SupperGreenColor;
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

            foreach (PAFocusModel pa in focuses)
            {
                var row = sheet.CreateRow(rowIndex);
                ICellStyle bodyStyleCenter = workbook.CreateCellStyle();
                ICellStyle bodyStyleLeft = workbook.CreateCellStyle();

                columnIndex = 0;
                var condition = pa.BonusGiven + pa.Contribution;
                if (condition >= MaxStone)
                {
                    if (pa.IsReadyPromotion.HasValue && pa.IsReadyPromotion.Value)
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

                var cellBody = row.CreateCell(columnIndex++);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.CellStyle.Alignment = HorizontalAlignment.Center;
                cellBody.SetCellValue(index++);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(pa.Employee.NickName);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleLeft;
                cellBody.SetCellValue(pa.Employee.FullName);
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

                var performance = pa.IsPoorPerformance.HasValue ? pa.IsPoorPerformance.Value ? Lang.Yes : Lang.No : Lang.Dash;
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
                sheet.SetColumnWidth(columnIndex++, columnWidth * 10);

                var score = pa.FinalScore.HasValue && pa.FinalScore.Value != 0 ? pa.FinalScore.ToString() : Lang.Dash;
                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle = bodyStyleCenter;
                cellBody.SetCellValue(score);
                sheet.SetColumnWidth(columnIndex++, columnWidth * 2);

                cellBody = row.CreateCell(columnIndex);
                cellBody.CellStyle.WrapText = true;
                cellBody.CellStyle = bodyStyleCenter;
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
            cell.BorderTop = BorderStyle.Thin;
            cell.BorderRight = BorderStyle.Thin;
            cell.BorderBottom = BorderStyle.Thin;
            cell.BorderLeft = BorderStyle.Thin;
        }
    }
}