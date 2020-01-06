namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Constants;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

#pragma warning disable S109 // Magic numbers should not be used
    public class ExportPAFocusService : IExportPAFocusService
    {
        private enum TemplateRowIndex
        {
            Even = 4,
            Odd,
            VeryGood,
            Outstanding,
            Poor
        };

        private const string ExcelTemplatePath = @"ExcelTemplates\\PAFocus.xlsx";
        private const int TotalCells = 11;
        private const int PAFocusColumnWidth = 60;

        private const int RowIndexColumn = 1;
        private const int NickNameColumn = 2;
        private const int FullNameColumn = 3;
        private const int JobTitleColumn = 4;
        private const int GroupNameColumn = 5;
        private const int ContributionColumn = 6;
        private const int BonusGivenColumn = 7;
        private const int ReadyPromotionColumn = 8;
        private const int PAGradeColumn = 9;
        private const int CurrentObjectiveColumn = 10;
        private const int NextObjectiveColumn = 11;

        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICommonService commonService;

        public ExportPAFocusService(
            IHostingEnvironment hostingEnvironment,
            ICommonService commonService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.commonService = commonService;
        }

        public async Task<byte[]> ExportAsync(PeriodBaseModel period, IEnumerable<PAFocusModel> paFocuses)
        {
            string[] refNames = { ReferenceResource.FocusEvaluationLevel };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));
            using (var templatePackage = new ExcelPackage(fileInfo))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets.First();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PA Focus Report");

                    // write header
                    const byte HeaderRowExpand = 3;
                    templateWorksheet.Cells[1, 1, HeaderRowExpand, TotalCells].Copy(worksheet.Cells[1, 1, HeaderRowExpand, TotalCells]);
                    worksheet.Cells[1, 1].Value = string.Format("PA Focus of {0}", period != null ? period.Name : string.Empty);

                    // write data
                    var rowIndex = 1;
                    var currentRow = 4;
                    foreach (PAFocusModel paFocus in paFocuses)
                    {
                        var isReadyPromotion = !string.IsNullOrWhiteSpace(paFocus.IsReadyPromotion) ? paFocus.IsReadyPromotion.ToLowerInvariant() : string.Empty;

                        TemplateRowIndex templateRowIndex = GetCellsTemplate(templateWorksheet, worksheet, paFocus, rowIndex, currentRow);

                        worksheet.Cells[currentRow, RowIndexColumn].Value = rowIndex;
                        worksheet.Cells[currentRow, NickNameColumn].Value = paFocus.Employee.NickName;
                        worksheet.Cells[currentRow, FullNameColumn].Value = paFocus.Employee.FullName;
                        worksheet.Cells[currentRow, JobTitleColumn].Value = paFocus.Employee.JobTitle;
                        worksheet.Cells[currentRow, GroupNameColumn].Value = paFocus.Employee.GroupName;
                        worksheet.Cells[currentRow, ContributionColumn].Value = GetFocusEvaluationLevel(referenceResources, paFocus.Contribution);
                        worksheet.Cells[currentRow, BonusGivenColumn].Value = GetFocusEvaluationLevel(referenceResources, paFocus.BonusGiven);
                        worksheet.Cells[currentRow, ReadyPromotionColumn].Value = GetIsReadyPromotion(isReadyPromotion);

                        WritePAGrade(worksheet.Cells[currentRow, PAGradeColumn], templateRowIndex, paFocus);

                        worksheet.Cells[currentRow, CurrentObjectiveColumn].Value = paFocus.CurrentObjective;
                        worksheet.Cells[currentRow, NextObjectiveColumn].Value = paFocus.NextObjective;

                        rowIndex++;
                        currentRow++;
                    }

                    worksheet.Column(NickNameColumn).AutoFit();
                    worksheet.Column(FullNameColumn).AutoFit();
                    worksheet.Column(JobTitleColumn).AutoFit();
                    worksheet.Column(GroupNameColumn).AutoFit();
                    worksheet.Column(ContributionColumn).AutoFit();
                    worksheet.Column(BonusGivenColumn).AutoFit();
                    worksheet.Column(ReadyPromotionColumn).AutoFit();
                    worksheet.Column(PAGradeColumn).AutoFit();
                    worksheet.Column(CurrentObjectiveColumn).Width = PAFocusColumnWidth;
                    worksheet.Column(NextObjectiveColumn).Width = PAFocusColumnWidth;

                    const byte BodyRowIndex = 4;
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    return package.GetAsByteArray();
                }
            }
        }

        private static TemplateRowIndex GetCellsTemplate(
            ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            PAFocusModel paFocus,
            int rowIndex,
            int currentRow)
        {
            TemplateRowIndex templateRowIndex;
            const byte MinStone = 3;
            const byte MaxStone = 9;
            int.TryParse(paFocus.BonusGiven, out int bonusGiven);
            int.TryParse(paFocus.Contribution, out int contribution);
            var condition = bonusGiven + contribution;
            var isReadyPromotion = !string.IsNullOrWhiteSpace(paFocus.IsReadyPromotion) ? paFocus.IsReadyPromotion.ToLowerInvariant() : string.Empty;

            if (condition >= MaxStone)
            {
                if (isReadyPromotion == "true")
                {
                    templateRowIndex = TemplateRowIndex.Outstanding;
                    templateWorksheet.Cells[(int)TemplateRowIndex.Outstanding, 1, (int)TemplateRowIndex.Outstanding, TotalCells]
                        .Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }
                else
                {
                    templateRowIndex = TemplateRowIndex.VeryGood;
                    templateWorksheet.Cells[(int)TemplateRowIndex.VeryGood, 1, (int)TemplateRowIndex.VeryGood, TotalCells]
                        .Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }
            }
            else if (bonusGiven > 0 && contribution > 0 && condition <= MinStone)
            {
                templateRowIndex = TemplateRowIndex.Poor;
                templateWorksheet.Cells[(int)TemplateRowIndex.Poor, 1, (int)TemplateRowIndex.Poor, TotalCells]
                    .Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
            }
            else
            {
                if (rowIndex % 2 == 0)
                {
                    templateRowIndex = TemplateRowIndex.Even;
                    templateWorksheet.Cells[(int)TemplateRowIndex.Even, 1, (int)TemplateRowIndex.Even, TotalCells]
                        .Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }
                else
                {
                    templateRowIndex = TemplateRowIndex.Odd;
                    templateWorksheet.Cells[(int)TemplateRowIndex.Odd, 1, (int)TemplateRowIndex.Odd, TotalCells]
                        .Copy(worksheet.Cells[currentRow, 1, currentRow, TotalCells]);
                }

                if (isReadyPromotion == "true")
                {
                    worksheet.Cells[currentRow, ReadyPromotionColumn].Style.Font.Color.SetColor(Color.Green);
                }
            }

            return templateRowIndex;
        }

        private static string GetIsReadyPromotion(string isReadyPromotion)
        {
            if (string.IsNullOrWhiteSpace(isReadyPromotion))
            {
                return "-";
            }

            return isReadyPromotion == "true" ? "Yes" : "No";
        }

        private static void WritePAGrade(ExcelRange gradeCell, TemplateRowIndex templateRowIndex, PAFocusModel paFocus)
        {
            int gradeId = paFocus.Grade != null ? paFocus.Grade.GradeId : Constants.MinGradeId;
            int originalGradeId = paFocus.OriginalGrade != null ? paFocus.OriginalGrade.GradeId : Constants.MinGradeId;

            gradeCell.Value = paFocus.Grade == null ? "N/A" : paFocus.Grade.Name;

            GetGradeIcon(gradeCell.RichText, templateRowIndex, originalGradeId, gradeId);
        }

        private static void GetGradeIcon(ExcelRichTextCollection richText, TemplateRowIndex templateRowIndex, int originalGradeId, int gradeId)
        {
            var icon = "";
            var color = Color.Black;
            if (gradeId > originalGradeId)
            {
                icon = "▲";
                color = (templateRowIndex == TemplateRowIndex.Even || templateRowIndex == TemplateRowIndex.Odd) ? Color.Green : Color.White;
            }
            else
            {
                if (gradeId < originalGradeId)
                {
                    icon = "▼";
                    color = (templateRowIndex == TemplateRowIndex.Even || templateRowIndex == TemplateRowIndex.Odd) ? Color.Red : Color.White;
                }
            }

            var iconText = richText.Add(icon);
            iconText.Bold = true;
            iconText.Color = color;
        }

        private static string GetFocusEvaluationLevel(IEnumerable<ReferenceResourceModel> referenceResources, string resourceId)
        {
            ReferenceResourceModel referenceResource = referenceResources.FirstOrDefault(x => x.ResourceId.ToString() == resourceId);

            return referenceResource != null ? referenceResource.Value : "-";
        }
    }
}