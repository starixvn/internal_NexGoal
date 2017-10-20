namespace NexGoal.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class EPADetailReportService : IEPADetailReportService
    {
        private HSSFWorkbook workbook;
        private ISheet sheet;
        private int _currentRow;
        private IList<string> columnsName;

        public IEnumerable<EPADetailModel> Details { get; set; }

        public MemoryStream Export(IEnumerable<EPADetailModel> epaDetails)
        {
            this.Details = epaDetails;
            workbook = new HSSFWorkbook();
            sheet = workbook.CreateSheet(Lang.EPADetailReportSheet);
            columnsName = CreateColumnsName();
            WriteTitleSheet();
            _currentRow += 2;

            WriteTitleTable();
            _currentRow++;

            WriteContentTable();

            var output = new MemoryStream();
            workbook.Write(output);

            return output;
        }

        public void Render(IEnumerable<EPADetailModel> epaDetail)
        {
            if (workbook == null)
            {
                Export(epaDetail);
            }

            HttpResponse response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}_{1}.xls", Lang.EPADetailReportName, DateTime.Now.ToString("yyyyMMddhhmmss")));

            workbook.Write(response.OutputStream);
        }

        private static IList<string> CreateColumnsName()
        {
            return new List<string>
            {
                Lang.SymbolCount,
                Lang.NickName,
                Lang.FullName,
                Lang.JobTitle,
                Lang.Team,
                Lang.EvaluationPeriod,
                Lang.PeriodType,
                Lang.SelfScore,
                Lang.EvaluationScore,
                Lang.ReviewerScore,
                Lang.FinalScore,
                Lang.Grade
            };
        }

        private static string CalculateSelfScore(EPADetailModel epaInfo)
        {
            var selfScore = string.Empty;

            if (!epaInfo.Process.Self.IsEndProcess)
            {
                selfScore = Lang.Pending;
            }
            else
            {
                if (string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore))
                {
                    selfScore = Lang.Dash;
                }
                else
                {
                    float score = epaInfo.Process.Self.TotalScore.Length > 0 ? float.Parse(epaInfo.Process.Self.TotalScore) : 0;

                    selfScore = Math.Round(score).ToString(CultureInfo.InvariantCulture);
                }
            }

            return selfScore;
        }

        private static string CalculateEvaluatorScore(EPADetailModel epaInfo)
        {
            var score = string.Empty;

            score = epaInfo.Process.Evaluators.Any() && epaInfo.Process.Evaluators[0].IsEndProcess
                ? epaInfo.Process.AverageScoreByEvaluators.Equals(0)
                    ? Lang.Dash
                    : Math.Round(epaInfo.Process.AverageScoreByEvaluators).ToString(CultureInfo.InvariantCulture)
                : Lang.Pending;

            return score;
        }

        private static string CalculateReviewerScore(EPADetailModel epaInfo)
        {
            var score = string.Empty;

            score = epaInfo.Process.Reviewers.Any() && epaInfo.Process.Reviewers[0].IsEndProcess
                ? epaInfo.Process.AverageScoreByReviewers.Equals(0)
                    ? Lang.Dash
                    : Math.Round(epaInfo.Process.AverageScoreByReviewers).ToString(CultureInfo.InvariantCulture)
                : Lang.Pending;

            return score;
        }

        private void WriteTitleSheet()
        {
            IRow rowTitle = sheet.CreateRow(_currentRow);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(Lang.EPADetailReportSheet);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(_currentRow, _currentRow, 0, columnsName.Count));
        }

        private void WriteTitleTable()
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

            IRow row = sheet.CreateRow(_currentRow);
            for (int i = 0; i < columnsName.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(columnsName[i]);
                sheet.AutoSizeColumn(i);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void WriteContentTable()
        {
            IFont redFont = workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

            ICellStyle evenStyle = CreateEvenStyle();
            ICellStyle oddStyle = CreateOddStyle();
            ICellStyle evenRedStyle = CreateEvenStyle();
            evenRedStyle.SetFont(redFont);
            ICellStyle oddRedStyle = CreateOddStyle();
            oddRedStyle.SetFont(redFont);

            var index = 1;
            foreach (var memberEPA in Details.OrderBy(i => i.Employee.NickName))
            {
                var currentColumn = 0;

                ICellStyle bodyStyle = _currentRow % 2 == 0 ? evenStyle : oddStyle;

                IRow row = sheet.CreateRow(_currentRow);
                dynamic score = CalculateScore(memberEPA);

                var cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(index++);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.NickName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.FullName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.JobTitleName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.GroupName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Period.Name);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Period.TargetTypeName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.SelfScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.EvaluatorScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.ReviewerScore);

                bodyStyle = _currentRow % 2 == 0 ? evenStyle : oddStyle;

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.FinalScore);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.Grade);

                _currentRow++;
            }
        }

        private dynamic CalculateScore(EPADetailModel epaInfo)
        {
            string selfScore = string.Empty;
            string evaluatorScore = string.Empty;
            string reviewerScore = string.Empty;
            string finalScore = string.Empty;
            string grade = string.Empty;

            if (epaInfo.EPAId > 0)
            {
                selfScore = CalculateSelfScore(epaInfo);
                evaluatorScore = CalculateEvaluatorScore(epaInfo);
                reviewerScore = CalculateReviewerScore(epaInfo);
            }

            if (!epaInfo.Period.IsEndPeriod)
            {
                finalScore = Lang.Pending;
                grade = Lang.Pending;
            }
            else
            {
                if (!string.IsNullOrEmpty(epaInfo.FinalScore) && epaInfo.FinalScore != "0")
                {
                    finalScore = Math.Round(float.Parse(epaInfo.FinalScore)).ToString(CultureInfo.InvariantCulture);
                    grade = epaInfo.Grade;
                }
                else
                {
                    finalScore = Lang.Dash;
                    grade = Lang.Dash;
                }
            }

            return new
            {
                SelfScore = selfScore,
                EvaluatorScore = evaluatorScore,
                ReviewerScore = reviewerScore,
                FinalScore = finalScore,
                Grade = grade
            };
        }

        private ICellStyle CreateOddStyle()
        {
            ICellStyle oddStyle = workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Center;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateEvenStyle()
        {
            ICellStyle evenStyle = workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Center;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }
    }
}