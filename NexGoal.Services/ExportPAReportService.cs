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
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Report;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class ExportPAReportService : IExportPAReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\PAReport.xlsx";
        private const string PADetailView = "Report/PADetail";
        private const string PDFStylePath = @"dist\\styles\\paDetail-pdf.css";
        private const int TotalColumns = 9;
        private const int TitleHeight = 25;

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

        public async  Task<byte[]> ExportExcel(GetPARecordMultiPeriodParam param)
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

                    templateWorksheet.Cells[2, 1, 3, TotalColumns].Copy(worksheet.Cells[2, 1, 3, TotalColumns]);
                    worksheet.Row(2).Height = TitleHeight;
                    worksheet.Row(3).Height = TitleHeight;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 50;
                    worksheet.Column(6).Width = 50;
                    worksheet.Column(7).Width = 50;
                    worksheet.Column(8).Width = 50;
                    worksheet.Column(9).Width = 50;

                    var currentRow = 4;

                    // write data
                    foreach (PARecordGroupByPeriod period in paRecords)
                    {
                        WriteCellForPeriod(templateWorksheet, worksheet, period, ref currentRow);
                    }

                    worksheet.View.FreezePanes(4, 1);

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
            templateWorksheet.Cells[5, 1, 5, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
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
            templateWorksheet.Cells[6, 1, 6, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
            worksheet.Cells[currentRow, 1].Value = groupPA.Group.Name;
            worksheet.Row(currentRow).Height = TitleHeight;
            worksheet.Cells[currentRow, 1, currentRow, TotalColumns].Merge = true;
            currentRow++;

            var index = 1;

            foreach (var pa in groupPA.PARecords)
            {
                templateWorksheet.Cells[11, 1, 11, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
                worksheet.Cells[currentRow, 1].Value = index;
                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[currentRow, 2].Value = pa.Employee.NickName;
                worksheet.Cells[currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[currentRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[currentRow, 3].Value = pa.Employee.JobTitle;
                worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[currentRow, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                WriteToeic(templateWorksheet, worksheet.Cells[currentRow, 4], pa.ToeicPolicy, pa.Toeic);
                worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, 5], pa.Processes, PerformanceProcess.Self);
                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, 6], pa.Processes, PerformanceProcess.ProjectManager);
                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, 7], pa.Processes, PerformanceProcess.Evaluator);
                WritePerformanceRecordByProcess(worksheet.Cells[currentRow, 8], pa.Processes, PerformanceProcess.Reviewer);
                WritePAFinalGrade(worksheet.Cells[currentRow, 9], pa);

                worksheet.Row(currentRow).Style.WrapText = true;
                currentRow++;
                index++;
            }
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
            if (paRecord.StatusId == (int)PARecordStatus.InProgress && !paRecord.IsEdited)
            {
                paCell.Value = string.Empty;
            }
            else
            {
                var richTextCollection = paCell.RichText;
                var assessorText = richTextCollection.Add(paRecord.Processes.FirstOrDefault(x => x.ProcessId == (int)PerformanceProcess.Reviewer)?.Evaluator.NickName);
                assessorText.Bold = true;

                richTextCollection.Add(Environment.NewLine);

                var gradeText = richTextCollection.Add($"Grade: {(string.IsNullOrWhiteSpace(paRecord.Grade?.Name) ? "N/A" : paRecord.Grade.Name)} ");
                gradeText.Bold = false;

                if (paRecord.GradeId != paRecord.OriginalGradeId)
                {
                    int.TryParse(paRecord.GradeId, out int gradeId);
                    int.TryParse(paRecord.OriginalGradeId, out int originalGradeId);

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

                if (!string.IsNullOrWhiteSpace(paRecord.Comment))
                {
                    richTextCollection.Add(Environment.NewLine);

                    var commentText = richTextCollection.Add(paRecord.Comment);
                    commentText.Bold = false;
                    commentText.Color = Color.Black;
                }
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

        public async Task<Tuple<string, byte[]>> ExportPADetailPDF(int actorId, int paRecordId)
        {
            var reportDetail = await reportService.GetPAReportDetailAsync(actorId, paRecordId).ConfigureAwait(false);

            var pdf = await GetPdfSetting(reportDetail).ConfigureAwait(false);
            var fileName = $"{reportDetail.Employee.NickName}_PA_{reportDetail.PARecord.Period.Name}.pdf";
            var file = converter.Convert(pdf);

            return new Tuple<string, byte[]>(fileName, file);
        }

        private async Task<HtmlToPdfDocument> GetPdfSetting(PAReportDetail reportDetail)
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