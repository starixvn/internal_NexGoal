namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using DinkToPdf;
    using DinkToPdf.Contracts;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Report;
    using NexGoal.Resources;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

#pragma warning disable S109 // Magic numbers should not be used
    public class ExportPAReportService : IExportPAReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\PAReport.xlsx";
        private const string PADetailView = "PAReport/Pdf";
        private const string PDFStylePath = @"dist\\styles\\paDetail-pdf.css";
        private const string RevisedIcon = "✔";
        private const int TotalColumns = 12;
        private const int TitleHeight = 25;
        private const int TemplateRow = 11;
        private const int GroupTemplateRow = 6;
        private const int PeriodTemplateRow = 5;
        private const int FirstHeaderRow = 2;
        private const int SecondHeaderRow = 3;

        private const int RowIndexColumn = 1;
        private const int NickNameColumn = 2;
        private const int JobTitleColumn = 3;
        private const int ToeicColumn = 4;
        private const int SelfColumn = 5;
        private const int ProjectManagerColumn = 6;
        private const int EvaluatorColumn = 7;
        private const int ReviewerColumn = 8;
        private const int PAGradeColumn = 9;
        private const int PAAdjustmentColumn = 10;
        private const int PACommentColumn = 11;
        private const int RevisedStatusColumn = 12;

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IPAReportService reportService;
        private readonly IViewRenderService viewRenderService;
        private readonly IConverter converter;

        public ExportPAReportService(IHostingEnvironment hostingEnvironment,
            IPAReportService reportService,
            IViewRenderService viewRenderService,
            IConverter converter)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.reportService = reportService;
            this.viewRenderService = viewRenderService;
            this.converter = converter;
        }

        public async Task<byte[]> ExportExcelAsync(GetPARecordMultiPeriodParam param)
        {
            var paRecords = await reportService.GetPAReportAsync(param).ConfigureAwait(false);
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));

            using (var templatePackage = new ExcelPackage(fileInfo))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets.First();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PA Report");

                    // write header
                    templateWorksheet.Cells[1, 1, 1, TotalColumns].Copy(worksheet.Cells[1, 1, 1, TotalColumns]);
                    worksheet.Cells[1, 1].Value = "PERFORMANCE APPRAISAL REPORT";
                    worksheet.Row(1).Height = TitleHeight;
                    worksheet.Cells[1, 1, 1, TotalColumns].Merge = true;

                    templateWorksheet.Cells[FirstHeaderRow, 1, SecondHeaderRow, TotalColumns]
                        .Copy(worksheet.Cells[FirstHeaderRow, 1, SecondHeaderRow, TotalColumns]);
                    worksheet.Row(FirstHeaderRow).Height = TitleHeight;
                    worksheet.Row(SecondHeaderRow).Height = TitleHeight;

                    worksheet.Column(RowIndexColumn).Width = 5;
                    worksheet.Column(NickNameColumn).Width = 15;
                    worksheet.Column(JobTitleColumn).Width = 30;
                    worksheet.Column(ToeicColumn).Width = 10;
                    worksheet.Column(SelfColumn).Width = 50;
                    worksheet.Column(ProjectManagerColumn).Width = 50;
                    worksheet.Column(EvaluatorColumn).Width = 50;
                    worksheet.Column(ReviewerColumn).Width = 50;
                    worksheet.Column(PAGradeColumn).Width = 10;
                    worksheet.Column(PAAdjustmentColumn).Width = 15;
                    worksheet.Column(PACommentColumn).Width = 40;
                    worksheet.Column(RevisedStatusColumn).Width = 10;

                    var currentRow = 4;

                    // write data
                    foreach (PARecordGroupByPeriod period in paRecords)
                    {
                        WriteCellForPeriod(templateWorksheet, worksheet, period, ref currentRow);
                    }

                    const byte BodyRowIndex = 4;
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    return package.GetAsByteArray();
                }
            }
        }

        private static void WriteCellForPeriod(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            PARecordGroupByPeriod period
            , ref int currentRow)
        {
            var periodName = period.Period.Name;
            if (period.Period.IsInProgress)
            {
                periodName += " - In Progress";
            }
            templateWorksheet.Cells[PeriodTemplateRow, 1, PeriodTemplateRow, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
            worksheet.Cells[currentRow, 1].Value = periodName;
            worksheet.Row(currentRow).Height = TitleHeight;
            worksheet.Cells[currentRow, 1, currentRow, TotalColumns].Merge = true;
            currentRow++;
            if (period.Groups != null)
            {
                foreach (var group in period.Groups)
                {
                    WriteCellForGroup(templateWorksheet, worksheet, group, ref currentRow);
                }
            }
            else
            {
                WriteNoRecordFound(worksheet, ref currentRow);
            }
        }

        private static void WriteNoRecordFound(ExcelWorksheet worksheet, ref int currentRow)
        {
            var row = worksheet.Cells[currentRow, 1, currentRow, TotalColumns];
            row.Merge = true;
            row.Value = ViewLang.NoRecordFound;
            row.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            currentRow++;
        }

        private static void WriteCellForGroup(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            PAReportModel groupPA
            , ref int currentRow)
        {
            templateWorksheet.Cells[GroupTemplateRow, 1, GroupTemplateRow, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
            worksheet.Cells[currentRow, 1].Value = groupPA.Group.Name;
            worksheet.Row(currentRow).Height = TitleHeight;
            worksheet.Cells[currentRow, 1, currentRow, TotalColumns].Merge = true;
            currentRow++;

            var index = 1;

            foreach (var pa in groupPA.PARecords)
            {
                templateWorksheet.Cells[TemplateRow, 1, TemplateRow, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
                worksheet.Cells[currentRow, RowIndexColumn].Value = index;
                worksheet.Cells[currentRow, RowIndexColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, RowIndexColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[currentRow, NickNameColumn].Value = pa.Employee.NickName;
                worksheet.Cells[currentRow, NickNameColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[currentRow, NickNameColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[currentRow, JobTitleColumn].Value = pa.Employee.JobTitle;
                worksheet.Cells[currentRow, JobTitleColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[currentRow, JobTitleColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                WriteToeic(templateWorksheet, worksheet.Cells[currentRow, ToeicColumn], pa.ToeicPolicy, pa.Toeic);
                worksheet.Cells[currentRow, ToeicColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, ToeicColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, SelfColumn], pa.Processes, PerformanceProcess.Self);
                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, ProjectManagerColumn], pa.Processes, PerformanceProcess.ProjectManager);
                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, EvaluatorColumn], pa.Processes, PerformanceProcess.Evaluator);
                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, ReviewerColumn], pa.Processes, PerformanceProcess.Reviewer);
                WritePAFinalGrade(worksheet.Cells[currentRow, PAGradeColumn], pa);
                WritePAAdjustment(worksheet.Cells[currentRow, PAAdjustmentColumn], pa);
                WritePAComment(worksheet.Cells[currentRow, PACommentColumn], pa);
                WriteRevisedStatus(worksheet.Cells[currentRow, RevisedStatusColumn], pa.IsRevised);

                worksheet.Row(currentRow).Style.WrapText = true;
                currentRow++;
                index++;
            }
        }

        private static void WriteRevisedStatus(ExcelRange cell, bool isRevised)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Size = 15;
            var richTextCollection = cell.RichText;
            richTextCollection.Add(isRevised ? RevisedIcon : string.Empty).Color = Color.Green;
        }

        private static void WriteToeic(ExcelWorksheet templateWorksheet, ExcelRange toeicCell, int toeicPolicy, int? toeic)
        {
            if (toeicPolicy != 0)
            {
                var coppyCell = toeicPolicy > 0 ? 8 : 9;
                templateWorksheet.Cells[coppyCell, 1].Copy(toeicCell);
            }

            toeicCell.Value = toeic != null ? toeic.ToString() : "N/A";
        }

        private static void WritePerformanceRecordByProcess(ExcelRange cell,
            IEnumerable<PAProcessModel> paProcesses,
            PerformanceProcess processId)
        {
            var processes = paProcesses.Where(p => p.ProcessId == (int)processId);

            if (processes.Any())
            {
                foreach (var process in processes)
                {
                    WritePerformanceRecordContent(cell, process.PerformanceRecord);
                }
            }
        }

        private static void WritePAFinalGrade(ExcelRange paCell, PARecordModel paRecord)
        {
            paCell.Value = paRecord.Grade != null ? paRecord.Grade.Name : "N/A";
        }

        private static void WritePAAdjustment(ExcelRange paCell, PARecordModel paRecord)
        {
            var grade = paRecord.Grade != null ? paRecord.Grade.GradeId : Constants.MinGradeId;
            var originalGrade = paRecord.OriginalGrade != null ? paRecord.OriginalGrade.GradeId : Constants.MinGradeId;

            if (grade > originalGrade)
            {
                paCell.Value = "▲";
                paCell.Style.Font.Color.SetColor(Color.Green);
            }

            if (grade < originalGrade)
            {
                paCell.Value = "▼";
                paCell.Style.Font.Color.SetColor(Color.Red);
            }
        }

        private static void WritePAComment(ExcelRange paCell, PARecordModel paRecord)
        {
            var richTextCollection = paCell.RichText;
            var assessorText = richTextCollection.Add(paRecord.Processes.FirstOrDefault(x => x.ProcessId == (int)PerformanceProcess.Reviewer)?.Evaluator.NickName);
            assessorText.Bold = true;

            if (!string.IsNullOrWhiteSpace(paRecord.Comment))
            {
                richTextCollection.Add(Environment.NewLine);
                richTextCollection.Add(Environment.NewLine);

                var commentText = richTextCollection.Add(paRecord.Comment);
                commentText.Bold = false;
            }
        }

        private static void WritePerformanceRecordContent(ExcelRange cell, PerformanceRecord record)
        {
            if (record != null)
            {
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                var richTextCollection = cell.RichText;
                var assessorText = richTextCollection.Add(record.Creator.NickName);
                assessorText.Bold = true;

                richTextCollection.Add(Environment.NewLine);

                var grade = record.Grade != null ? record.Grade.Name : "N/A";
                var gradeText = richTextCollection.Add($"Grade: {grade} ");
                gradeText.Bold = false;

                richTextCollection.Add(Environment.NewLine);
                richTextCollection.Add(record.Comment);
                richTextCollection.Add(Environment.NewLine);
                richTextCollection.Add(Environment.NewLine);
            }
        }

        public async Task<Tuple<string, byte[]>> ExportPdfAsync(PAReportDetail reportDetail)
        {
            var pdf = await GetPdfSettingAsync(reportDetail).ConfigureAwait(false);
            var fileName = $"{reportDetail.Employee.NickName}_PA_{reportDetail.PARecord.Period.Name}.pdf";
            var file = converter.Convert(pdf);

            return new Tuple<string, byte[]>(fileName, file);
        }

        private async Task<HtmlToPdfDocument> GetPdfSettingAsync(PAReportDetail reportDetail)
        {
            string htmlString = await viewRenderService.RenderToStringAsync(PADetailView, reportDetail).ConfigureAwait(false);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Unit = Unit.Millimeters, Top = 10, Bottom = 10, Left = 4, Right = 4 },
                DocumentTitle = $"{reportDetail.Employee.NickName}'s_PA"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlString,
                WebSettings = {
                    DefaultEncoding = "utf-8",
                    Background = true,
                    LoadImages = true,
                   UserStyleSheet = Path.Combine(hostingEnvironment.WebRootPath, PDFStylePath)
                },
                FooterSettings = { FontName = "Arial", FontSize = 7, Spacing = 1.5, Center = "[page]" }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return pdf;
        }
    }
}