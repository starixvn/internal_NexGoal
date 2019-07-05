namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Probation;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;
    using System.Threading.Tasks;

    [NexGoalPermission(FunctionKey = FunctionKey.ProbationReport, Policy = Policy.Read)]
    public class ProbationReportController : BaseController
    {
        private readonly IProbationReportService probationReportService;

        public ProbationReportController(IProbationReportService probationReportService, ISessionService sessionService) : base(sessionService)
        {
            this.probationReportService = probationReportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsAndEmployees()
        {
            var result = await probationReportService.GetGroupsAndEmployeesAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(new { groups = result.Item1, employees = result.Item2 });
        }

        [HttpGet]
        public async Task<IActionResult> Get(GetProbationReportViewModel model)
        {
            var result = await probationReportService.GetAsync(new GetProbationReportModel
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeIds = model.EmployeeIds,
                IsQualified = model.IsQualified,
                IsQualifiedByEvaluator = model.IsQualifiedByEvaluator,
                ActorId = sessionService.EmployeeId
            }).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(GetProbationReportViewModel model)
        {
            var fileContent = await probationReportService.ExportExcelAsync(new GetProbationReportModel
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeIds = model.EmployeeIds,
                IsQualified = model.IsQualified,
                IsQualifiedByEvaluator = model.IsQualifiedByEvaluator,
                ActorId = sessionService.EmployeeId
            }).ConfigureAwait(false);

            return File(fileContent, MimeType.XLSX, "Probation Report.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var pdfModel = await probationReportService.ExportPdfAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            if (pdfModel != null)
            {
                return File(pdfModel.Content, MimeType.PDF, pdfModel.FileName);
            }

            return RedirectToAction("PageNotFound", "Error");
        }
    }
}