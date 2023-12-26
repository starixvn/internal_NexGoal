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
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Probation;
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;
    using NexGoal.Repositories;
    using OfficeOpenXml;

    public class ProbationReportService : IProbationReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\Probation Report.xlsx";
        private const string PDFViewName = "ProbationReport/Pdf";
        private const string PDFStylePath = @"dist\\styles\\probation-report-pdf.css";
        private Color passedColor = Color.FromArgb(1, 67, 135, 0);
        private Color failedColor = Color.FromArgb(1, 226, 44, 4);

        private const int OrderColumn = 1;
        private const int NickNameColumn = 2;
        private const int FullNameColumn = 3;
        private const int JoinedDateColumn = 4;
        private const int GroupNameColumn = 5;
        private const int JobTitleColumn = 6;
        private const int SelfScoreColumn = 7;
        private const int SelfCommentColumn = 8;
        private const int EvaluatorScoreColumn = 9;
        private const int EvaluatorResultColumn = 10;
        private const int EvaluatorCommentColumn = 11;
        private const int ReviewerResultColumn = 12;
        private const int ReviewerCommentColumn = 13;

        private readonly IProbationRepository probationRepository;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IViewRenderService viewRenderService;
        private readonly IConverter converter;
        private readonly ICommonService commonService;

        public ProbationReportService(IProbationRepository probationRepository, IBaseService baseService, ICryptoService cryptoService,
            IWebHostEnvironment hostingEnvironment,
             IViewRenderService viewRenderService,
            IConverter converter,
            ICommonService commonService)
        {
            this.probationRepository = probationRepository;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.hostingEnvironment = hostingEnvironment;
            this.viewRenderService = viewRenderService;
            this.converter = converter;
            this.commonService = commonService;
        }

        public async Task<ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndEmployeesAsync(int actorId)
        {
            return await probationRepository.GetGroupsAndEmployeesAsync(new GetGroupsAndEmployeesCriteria
            {
                ActorId = actorId
            });
        }

        public async Task<IEnumerable<ProbationReportModel>> GetAsync(GetProbationReportModel model)
        {
            var result = await baseService.GetDataAsync<ProbationReportModel>(new GetProbationReportCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeIds = model.EmployeeIds,
                IsQualified = model.IsQualified,
                IsQualifiedByEvaluator = model.IsQualifiedByEvaluator,
                ActorId = model.ActorId
            });

            foreach (var probationer in result)
            {
                probationer.SelfScore = cryptoService.Decrypt(probationer.SelfScore);
                probationer.EvaluatorScore = cryptoService.Decrypt(probationer.EvaluatorScore);
            }

            return result;
        }

        public async Task<byte[]> ExportExcelAsync(GetProbationReportModel model)
        {
            var data = await GetAsync(model);

            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, ExcelTemplatePath));
            using (var package = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                var currentRow = 4;
                var position = 1;

                FillDate(model, worksheet);

                foreach (var probationer in data)
                {
                    var selfComment = commonService.RemoveInvalidXmlChars(probationer.SelfComment);
                    var evaluatorComment = commonService.RemoveInvalidXmlChars(probationer.EvaluatorComment);
                    var reviewerComment = commonService.RemoveInvalidXmlChars(probationer.ReviewerComment);

                    worksheet.Cells[currentRow, OrderColumn].Value = position;
                    worksheet.Cells[currentRow, NickNameColumn].Value = probationer.NickName;
                    worksheet.Cells[currentRow, FullNameColumn].Value = probationer.FullName;
                    worksheet.Cells[currentRow, JoinedDateColumn].Value = probationer.JoinedDate.ToString("dd MMM, yyyy");
                    worksheet.Cells[currentRow, GroupNameColumn].Value = probationer.GroupName;
                    worksheet.Cells[currentRow, JobTitleColumn].Value = probationer.JobTitle;
                    worksheet.Cells[currentRow, SelfScoreColumn].Value = probationer.SelfScore;
                    worksheet.Cells[currentRow, SelfCommentColumn].Value = selfComment;
                    worksheet.Cells[currentRow, EvaluatorScoreColumn].Value = probationer.EvaluatorScore;

                    FillEvaluatorResult(probationer, worksheet.Cells[currentRow, EvaluatorResultColumn]);

                    worksheet.Cells[currentRow, EvaluatorCommentColumn].Value = evaluatorComment;

                    FillReviewerResult(probationer, worksheet.Cells[currentRow, ReviewerResultColumn]);

                    worksheet.Cells[currentRow, ReviewerCommentColumn].Value = reviewerComment;

                    position++;
                    currentRow++;
                }

                return package.GetAsByteArray();
            }
        }

        public async Task<PdfModel> ExportPdfAsync(int employeeId, int actorId)
        {
            var reportModel = await GetPdfData(employeeId, actorId);

            if (reportModel != null)
            {
                var pdf = await GetPdfSettingAsync(reportModel);

                return new PdfModel
                {
                    FileName = $"{reportModel.NickName}_Probation.pdf",
                    Content = converter.Convert(pdf)
                };
            }

            return null;
        }

        private async Task<HtmlToPdfDocument> GetPdfSettingAsync(ProbationReportPdfModel reportModel)
        {
            string htmlString = await viewRenderService.RenderToStringAsync(PDFViewName, reportModel);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Unit = Unit.Millimeters, Top = 10, Bottom = 10, Left = 4, Right = 4 },
                DocumentTitle = $"Probation of {reportModel.NickName}"
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

        public async Task<ValueTuple<ProbationReportModel, IEnumerable<KPIGroupModel>, HRMModel>> GetByEmployeeIdAsync(int employeeId, int actorId)
        {
            var result = await baseService.FetchMultipleAsync<ProbationReportModel, KPIGroupModel, EncryptedProbationEvaluationDetailModel>(
                new GetProbationReportByEmployeeIdCriteria { EmployeeId = employeeId, ActorId = actorId });

            var probationer = result.Item1.FirstOrDefault();
            var groups = result.Item2;
            var details = result.Item3;

            if (probationer != null)
            {
                foreach (var group in groups)
                {
                    group.Details = details.Where(x => x.KPIGroupId == group.KPIGroupId).Select(x => new ProbationEvaluationDetailModel
                    {
                        KPIId = x.KPIId,
                        KPIName = x.KPIName,
                        KPIGroupId = x.KPIGroupId,
                        EmployeeId = x.EmployeeId,
                        SelfPoint = string.IsNullOrWhiteSpace(x.SelfPoint) ? (decimal?)null : decimal.Parse(cryptoService.Decrypt(x.SelfPoint)),
                        EvaluatorPoint = string.IsNullOrWhiteSpace(x.EvaluatorPoint) ? (decimal?)null : decimal.Parse(cryptoService.Decrypt(x.EvaluatorPoint))
                    });
                }

                probationer.SelfScore = cryptoService.Decrypt(probationer.SelfScore);
                probationer.EvaluatorScore = cryptoService.Decrypt(probationer.EvaluatorScore);
                var hrm = await probationRepository.GetHRM(new GetHRMCriteria { EmployeeId = employeeId });
                return new (probationer, groups, hrm);
            }

            return default;
        }

        private async Task<ProbationReportPdfModel> GetPdfData(int employeeId, int actorId)
        {
            var result = await baseService.FetchMultipleAsync<ProbationReportPdfModel, KPIGroupModel, EncryptedProbationEvaluationDetailModel>(
                new GetProbationReportByEmployeeIdCriteria { EmployeeId = employeeId, ActorId = actorId });

            var probationer = result.Item1.FirstOrDefault();

            if (probationer != null)
            {
                var groups = result.Item2;
                var details = result.Item3;

                foreach (var group in groups)
                {
                    group.Details = details.Where(x => x.KPIGroupId == group.KPIGroupId).Select(x => new ProbationEvaluationDetailModel
                    {
                        KPIId = x.KPIId,
                        KPIName = x.KPIName,
                        KPIGroupId = x.KPIGroupId,
                        EmployeeId = x.EmployeeId,
                        SelfPoint = string.IsNullOrWhiteSpace(x.SelfPoint) ? (decimal?)null : decimal.Parse(cryptoService.Decrypt(x.SelfPoint)),
                        EvaluatorPoint = string.IsNullOrWhiteSpace(x.EvaluatorPoint) ? (decimal?)null : decimal.Parse(cryptoService.Decrypt(x.EvaluatorPoint))
                    });
                }

                probationer.SelfScore = cryptoService.Decrypt(probationer.SelfScore);
                probationer.EvaluatorScore = cryptoService.Decrypt(probationer.EvaluatorScore);

                probationer.Details = groups;

                probationer.Hrm = await probationRepository.GetHRM(new GetHRMCriteria { EmployeeId = employeeId });
            }
            return probationer;
        }

        private static void FillDate(GetProbationReportModel model, ExcelWorksheet worksheet)
        {
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                var titleCell = worksheet.Cells[1, 1];
                titleCell.Value = $"{titleCell.Value} ({model.FromDate.Value.ToString("dd MMM, yyyy")} - {model.ToDate.Value.ToString("dd MMM, yyyy")})";
            }
        }

        private void FillReviewerResult(ProbationReportModel probationer, ExcelRange cell)
        {
            if (probationer.IsQualified.HasValue)
            {
                cell.Value = probationer.IsQualified.Value ? "Passed" : "Failed";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(probationer.IsQualified.Value ? passedColor : failedColor);
            }
        }

        private void FillEvaluatorResult(ProbationReportModel probationer, ExcelRange cell)
        {
            if (probationer.IsQualifiedByEvaluator.HasValue)
            {
                cell.Value = probationer.IsQualifiedByEvaluator.Value ? "Passed" : "Failed";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(probationer.IsQualifiedByEvaluator.Value ? passedColor : failedColor);
            }
        }
    }
}