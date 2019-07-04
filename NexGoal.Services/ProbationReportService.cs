namespace NexGoal.Services
{
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Probation;
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;
    using NexGoal.Repositories;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProbationReportService : IProbationReportService
    {
        private const string ExcelTemplatePath = @"ExcelTemplates\\Probation Report.xlsx";

        private readonly IProbationRepository probationRepository;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IHostingEnvironment hostingEnvironment;

        public ProbationReportService(IProbationRepository probationRepository, IBaseService baseService, ICryptoService cryptoService
            , IHostingEnvironment hostingEnvironment)
        {
            this.probationRepository = probationRepository;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndEmployeesAsync(int actorId)
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
                var passedColor = Color.FromArgb(1, 67, 135, 0);
                var failedColor = Color.FromArgb(1, 226, 44, 4);

                if (model.FromDate.HasValue && model.ToDate.HasValue)
                {
                    var titleCell = worksheet.Cells[1, 1];
                    titleCell.Value = $"{titleCell.Value} ({model.FromDate.Value.ToString("dd MMM, yyyy")} - {model.ToDate.Value.ToString("dd MMM, yyyy")})";
                }

                foreach (var probationer in data)
                {
                    worksheet.Cells[currentRow, 1].Value = position;
                    worksheet.Cells[currentRow, 2].Value = probationer.NickName;
                    worksheet.Cells[currentRow, 3].Value = probationer.FullName;
                    worksheet.Cells[currentRow, 4].Value = probationer.JoinedDate.ToString("dd MMM, yyyy");
                    worksheet.Cells[currentRow, 5].Value = probationer.GroupName;
                    worksheet.Cells[currentRow, 6].Value = probationer.JobTitle;
                    worksheet.Cells[currentRow, 7].Value = probationer.SelfScore;
                    worksheet.Cells[currentRow, 8].Value = probationer.SelfComment;
                    worksheet.Cells[currentRow, 9].Value = probationer.EvaluatorScore;

                    if (probationer.IsQualifiedByEvaluator.HasValue)
                    {
                        var cell = worksheet.Cells[currentRow, 10];
                        cell.Value = probationer.IsQualifiedByEvaluator.Value ? "Passed" : "Failed";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Color.SetColor(probationer.IsQualifiedByEvaluator.Value ? passedColor : failedColor);
                    }

                    worksheet.Cells[currentRow, 11].Value = probationer.EvaluatorComment;

                    if (probationer.IsQualified.HasValue)
                    {
                        var cell = worksheet.Cells[currentRow, 12];
                        cell.Value = probationer.IsQualified.Value ? "Passed" : "Failed";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Color.SetColor(probationer.IsQualified.Value ? passedColor : failedColor);
                    }

                    worksheet.Cells[currentRow, 13].Value = probationer.ReviewerComment;

                    position++;
                    currentRow++;
                }

                return package.GetAsByteArray();
            }
        }
    }
}