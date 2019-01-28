namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Report;
    using NexGoal.Services.Interfaces;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class ExportPAReportService : IExportPAReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\PAReport.xlsx";
        private const int TotalColumns = 9;

        private readonly IHostingEnvironment hostingEnvironment;

        public ExportPAReportService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
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
                    templateWorksheet.Cells[1, 1, 1, TotalColumns].Copy(worksheet.Cells[1, 1, 1, TotalColumns]);
                    worksheet.Cells[1, 1].Value = string.Format("PA Report of {0} period(s)", paRecords.Count());
                    worksheet.Cells[1, 1, 1, TotalColumns].Merge = true;
                    templateWorksheet.Cells[2, 1, 3, TotalColumns].Copy(worksheet.Cells[2, 1, 3, TotalColumns]);

                    var currentRow = 4;

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 50;
                    worksheet.Column(6).Width = 50;
                    worksheet.Column(7).Width = 50;
                    worksheet.Column(8).Width = 50;
                    worksheet.Column(9).Width = 50;
                    worksheet.View.FreezePanes(4, 1);

                    // write data
                    foreach (PARecordGroupByPeriod period in paRecords)
                    {
                        WriteCellForPeriod(templateWorksheet, worksheet, period, ref currentRow);
                    }

                    return package.GetAsByteArray();
                }
            }
        }

        private static void WriteCellForPeriod(ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            PARecordGroupByPeriod period
            , ref int currentRow)
        {
            templateWorksheet.Cells[5, 1, 5, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
            worksheet.Cells[currentRow, 1].Value = period.Period.Name;
            worksheet.Cells[currentRow, 1, currentRow, TotalColumns].Merge = true;
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
            templateWorksheet.Cells[6, 1, 6, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
            worksheet.Cells[currentRow, 1].Value = groupPA.Group.Name;
            worksheet.Cells[currentRow, 1, currentRow, TotalColumns].Merge = true;
            currentRow++;

            var index = 1;

            foreach (var pa in groupPA.PARecords)
            {
                templateWorksheet.Cells[11, 1, 11, TotalColumns].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalColumns]);
                worksheet.Cells[currentRow, 1].Value = index;
                worksheet.Cells[currentRow, 2].Value = pa.Employee.NickName;
                worksheet.Cells[currentRow, 3].Value = pa.Employee.JobTitle;
                SetToeicColor(currentRow, templateWorksheet, worksheet, pa.ToeicPolicy);
                worksheet.Cells[currentRow, 4].Value = pa.Toeic;
                WritePerformanceRecordByProcess(pa.Processes, PerformanceProcess.Self, worksheet, currentRow, 5);
                WritePerformanceRecordByProcess(pa.Processes, PerformanceProcess.ProjectManager, worksheet, currentRow, 6);
                WritePerformanceRecordByProcess(pa.Processes, PerformanceProcess.Evaluator, worksheet, currentRow, 7);
                WritePerformanceRecordByProcess(pa.Processes, PerformanceProcess.Reviewer, worksheet, currentRow, 8);

                var paPerformanceRecord = new PerformanceRecordModel
                {
                    Comment = pa.Comment,
                    Grade = pa.Grade,
                    Creator = pa.Processes.FirstOrDefault(p => p.ProcessId == (int)PerformanceProcess.Reviewer).Evaluator
                };
                worksheet.Row(currentRow).Style.WrapText = true;
                worksheet.Cells[currentRow, 9].Value = WritePerformanceRecordContent(paPerformanceRecord);
                currentRow++;
                index++;
            }
        }

        private static void SetToeicColor(int currentRow, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int toeicPolicy)
        {
            if (toeicPolicy != 0)
            {
                var coppyCell = toeicPolicy > 0 ? 8 : 9;
                templateWorksheet.Cells[coppyCell, 1].Copy(worksheet.Cells[currentRow, 4]);
            }
        }

        private static void WritePerformanceRecordByProcess(IEnumerable<PAProcessModel> processes,
            PerformanceProcess processId,
            ExcelWorksheet worksheet,
            int currentRow,
            int column)
        {
            string value = string.Empty;

            if (processId != PerformanceProcess.ProjectManager)
            {
                var process = processes.FirstOrDefault(p => p.ProcessId == (int)processId);
                if (process.PerformanceRecord != null)
                {
                    value = WritePerformanceRecordContent(process.PerformanceRecord);
                }
            }
            else
            {
                var pmProcesses = processes.Where(p => p.ProcessId == (int)PerformanceProcess.ProjectManager);
                foreach (var process in pmProcesses)
                {
                    var pm = WritePerformanceRecordContent(process.PerformanceRecord);
                    value = string.Format("{0}\n\n", pm);
                }
            }
            worksheet.Cells[currentRow, column].Value = value;
            worksheet.Cells[currentRow, column].Style.WrapText = true;
            worksheet.Cells[currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[currentRow, column].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        }

        private static string WritePerformanceRecordContent(PerformanceRecordModel record)
        {
            var value = string.Empty;
            var grade = record.Grade != null ? record.Grade.Name : "N/A";
            var nickName = record.Creator.NickName;
            var comment = record.Comment;
            value = string.Format("- {0}\nGrade: {1}\n {2}", nickName, grade, comment);
            return value;
        }
    }
}