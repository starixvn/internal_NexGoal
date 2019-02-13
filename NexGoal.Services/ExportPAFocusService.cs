namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Constants;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using OfficeOpenXml;

    public class ExportPAFocusService : IExportPAFocusService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\PAFocus.xlsx";
        private const int TotalCells = 11;

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICommonService commonService;

        public ExportPAFocusService(
            IHostingEnvironment hostingEnvironment,
            ICommonService commonService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.commonService = commonService;
        }

        public byte[] Export(PeriodBaseModel period, IEnumerable<PAFocusModel> paFocuses)
        {
            string[] refNames = { ReferenceResource.FocusEvaluationLevel };
            IEnumerable<ReferenceResourceModel> referenceResources = commonService.GetReferenceResourcesByNamesAsync(refNames).Result;
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));

            using (var templatePackage = new ExcelPackage(fileInfo))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets.First();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PA Focus Report");

                    // write header
                    templateWorksheet.Cells[1, 1, 3, TotalCells].Copy(worksheet.Cells[1, 1, 3, TotalCells]);
                    worksheet.Cells[1, 1].Value = string.Format("PA Focus of {0}", period != null ? period.Name : string.Empty);

                    // write data
                    var rowIndex = 1;
                    var currentRow = 4;
                    foreach (PAFocusModel paFocus in paFocuses)
                    {
                        var isReadyPromotion = !string.IsNullOrWhiteSpace(paFocus.IsReadyPromotion) ? paFocus.IsReadyPromotion.ToLowerInvariant() : string.Empty;

                        GetCellsTemplate(templateWorksheet, worksheet, paFocus, rowIndex, currentRow);

                        worksheet.Cells[currentRow, 1].Value = rowIndex;
                        worksheet.Cells[currentRow, 2].Value = paFocus.Employee.NickName;
                        worksheet.Cells[currentRow, 3].Value = paFocus.Employee.FullName;
                        worksheet.Cells[currentRow, 4].Value = paFocus.Employee.JobTitle;
                        worksheet.Cells[currentRow, 5].Value = paFocus.Employee.GroupName;
                        worksheet.Cells[currentRow, 6].Value = GetFocusEvaluationLevel(referenceResources, paFocus.Contribution);
                        worksheet.Cells[currentRow, 7].Value = GetFocusEvaluationLevel(referenceResources, paFocus.BonusGiven);
                        worksheet.Cells[currentRow, 8].Value = GetIsReadyPromotion(isReadyPromotion);
                        worksheet.Cells[currentRow, 9].Value = GetGrade(paFocus.Grade);
                        worksheet.Cells[currentRow, 10].Value = paFocus.CurrentObjective;
                        worksheet.Cells[currentRow, 11].Value = paFocus.NextObjective;

                        rowIndex++;
                        currentRow++;
                    }

                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();
                    worksheet.Column(10).Width = 60;
                    worksheet.Column(11).Width = 60;

                    worksheet.View.FreezePanes(4, 1);

                    return package.GetAsByteArray();
                }
            }
        }

        private static void GetCellsTemplate(
            ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            PAFocusModel paFocus,
            int rowIndex,
            int currentRow)
        {
            var evenRow = 4;
            var oddRow = 5;
            var veryGoodRow = 6;
            var outstandingRow = 7;
            var poorRow = 8;
            var minStone = 3;
            var maxStone = 9;
            int.TryParse(paFocus.BonusGiven, out int bonusGiven);
            int.TryParse(paFocus.Contribution, out int contribution);
            var condition = bonusGiven + contribution;
            var isReadyPromotion = !string.IsNullOrWhiteSpace(paFocus.IsReadyPromotion) ? paFocus.IsReadyPromotion.ToLowerInvariant() : string.Empty;

            if (condition >= maxStone)
            {
                if (isReadyPromotion == "true")
                {
                    templateWorksheet.Cells[outstandingRow, 1, outstandingRow, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }
                else
                {
                    templateWorksheet.Cells[veryGoodRow, 1, veryGoodRow, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }
            }
            else if (bonusGiven > 0 && contribution > 0 && condition <= minStone)
            {
                templateWorksheet.Cells[poorRow, 1, poorRow, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
            }
            else
            {
                if (rowIndex % 2 == 0)
                {
                    templateWorksheet.Cells[evenRow, 1, evenRow, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }
                else
                {
                    templateWorksheet.Cells[oddRow, 1, oddRow, TotalCells].Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }

                if (isReadyPromotion == "true")
                {
                    worksheet.Cells[currentRow, 8].Style.Font.Color.SetColor(Color.Green);
                }
            }
        }

        private static string GetIsReadyPromotion(string isReadyPromotion)
        {
            if (string.IsNullOrWhiteSpace(isReadyPromotion))
            {
                return "-";
            }

            return isReadyPromotion == "true" ? "Yes" : "No";
        }

        private static string GetGrade(PerformanceGrade performanceGrade)
        {
            return performanceGrade != null ? performanceGrade.Name : string.Empty;
        }

        private static string GetFocusEvaluationLevel(IEnumerable<ReferenceResourceModel> referenceResources, string resourceId)
        {
            ReferenceResourceModel referenceResource = referenceResources.FirstOrDefault(x => x.ResourceId.ToString() == resourceId);

            return referenceResource != null ? referenceResource.Value : "-";
        }
    }
}