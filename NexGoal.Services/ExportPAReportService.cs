namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using iTextSharp.text;
    using iTextSharp.text.html;
    using iTextSharp.text.html.simpleparser;
    using iTextSharp.text.pdf;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;
    using NexGoal.Services.Interfaces;
    using OfficeOpenXml;

    public class ExportPAReportService : IExportPAReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\PAReport.xlsx";
        private const string PdfTemplatePath = @"wwwroot\\EmailTemplates\\PaDetail.cshtml";
        private const int TotalCells = 8;

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IViewRenderService viewRenderService;

        public ExportPAReportService(
            IHostingEnvironment hostingEnvironment,
            IViewRenderService viewRenderService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.viewRenderService = viewRenderService;
        }

        public byte[] ExportExcel(IEnumerable<PARecordGroupByPeriod> paRecords)
        {
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));

            using (var templatePackage = new ExcelPackage(fileInfo))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets.First();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PA Report");

                    // write header
                    templateWorksheet.Cells[1, 1, 3, TotalCells].Copy(worksheet.Cells[1, 1, 3, TotalCells]);
                    worksheet.Cells[1, 1].Value = string.Format("PA Report of {0} period(s)", paRecords.Count());
                    var currentRow = 4;
                    // write data
                    foreach (PARecordGroupByPeriod period in paRecords)
                    {
                        WriteCellForPeriod(templateWorksheet, worksheet, period, ref currentRow);
                    }

                    worksheet.Column(1).Width = 20;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 15;

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
            templateWorksheet.Cells[5, 1, 5, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
            worksheet.Cells[currentRow, 1].Value = period.Period.Name;
            currentRow++;
            foreach (var group in period.Groups)
            {
                WriteCellForGroup(templateWorksheet, worksheet, group, ref currentRow);
            }
        }

        private static void WriteCellForGroup(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            PAReportModel groupPA
            , ref int currentRow)
        {
            templateWorksheet.Cells[6, 1, 6, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
            worksheet.Cells[currentRow, 1].Value = groupPA.Group.Name;
            currentRow++;

            foreach (var pa in groupPA.PARecords)
            {
                templateWorksheet.Cells[4, 1, 4, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                worksheet.Cells[currentRow, 1].Value = pa.Employee.NickName;
                worksheet.Cells[currentRow, 2].Value = pa.Employee.FullName;
                worksheet.Cells[currentRow, 3].Value = pa.Employee.JobTitle;
                worksheet.Cells[currentRow, 4].Value = pa.Toeic;
                worksheet.Cells[currentRow, 5].Value = WriteGradeByProcess(pa.Processes, PerformanceProcess.Self);
                worksheet.Cells[currentRow, 6].Value = WriteGradeByProcess(pa.Processes, PerformanceProcess.ProjectManager);
                worksheet.Cells[currentRow, 7].Value = WriteGradeByProcess(pa.Processes, PerformanceProcess.Evaluator);
                worksheet.Cells[currentRow, 8].Value = pa.Grade != null ? pa.Grade.Name : "N/A";

                currentRow++;
            }
        }

        private static string WriteGradeByProcess(IEnumerable<PAProcessModel> processes, PerformanceProcess processId)
        {
            var grade = string.Empty;
            if (processId != PerformanceProcess.ProjectManager)
            {
                var process = processes.FirstOrDefault(p => p.ProcessId == (int)processId);
                grade = process.PerformanceRecord != null && process.PerformanceRecord.Grade != null ? process.PerformanceRecord.Grade.Name : "N/A";
            }
            else
            {
                var grades = new List<string>();
                var pmProcesses = processes.Where(p => p.ProcessId == (int)PerformanceProcess.ProjectManager);
                foreach (var process in pmProcesses)
                {
                    var gradePerPM = CalculateGrade(process);
                    grades.Add($"{process.Evaluator.NickName}: {gradePerPM}");
                }
                grade = string.Join("; ", grades.ToArray());
            }

            return grade;
        }

        private static string CalculateGrade(PAProcessModel process)
        {
            return process.PerformanceRecord != null && process.PerformanceRecord.Grade != null ? process.PerformanceRecord.Grade.Name : "N/A";
        }

        public byte[] ExportPADetail(PARecordModel record)
        {
            var htmlText = viewRenderService.Render(PdfTemplatePath, record);
            var paDetail = RenderPDF(htmlText);
            return paDetail;
        }

        private static byte[] RenderPDF(string htmlText)
        {
            byte[] content = RenderContent(htmlText);

            using (var input = new MemoryStream(content))
            {
                using (var output = new MemoryStream())
                {
                    var reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, "", "", PdfWriter.ALLOW_SCREENREADERS);

                    return output.ToArray();
                }
            }
        }

        private static byte[] RenderContent(string htmlText)
        {
            using (var stream = new MemoryStream())
            {
                var pdfDocument = new Document(PageSize.A4);
                pdfDocument.SetPageSize(PageSize.A4);

                PdfWriter.GetInstance(pdfDocument, stream);
                pdfDocument.Open();

                var styles = new StyleSheet();
                styles.LoadTagStyle(HtmlTags.SPAN, "color", "red");
                var workers = HtmlWorker.ParseToList(new StringReader(htmlText), styles);

                foreach (IElement element in workers)
                {
                    pdfDocument.Add(element);
                }

                pdfDocument.Close();

                return stream.ToArray();
            }
        }
    }
}