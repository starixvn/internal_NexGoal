namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Probation;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;
    using System.Linq;
    using System.Threading.Tasks;

    [NexGoalPermission(FunctionKey = FunctionKey.ProbationReport, Policy = Policy.Read)]
    public class ProbationReportController : BaseController
    {
        private readonly IProbationReportService probationReportService;
        private readonly IEmployeeService employeeService;

        public ProbationReportController(IProbationReportService probationReportService,
            IEmployeeService employeeService,
            ISessionService sessionService) : base(sessionService)
        {
            this.probationReportService = probationReportService;
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await employeeService.GetRolesByEmployeeIdAsync(sessionService.EmployeeId).ConfigureAwait(false);
            return View(roles.Where(x => x.HasRole).Select(x => x.RoleId));
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            return View(id);
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
        public async Task<IActionResult> GetByEmployeeId(int id)
        {
            var result = await probationReportService.GetByEmployeeIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);
            if (result != null)
            {
                return Json(new { probationer = result.Item1, details = result.Item2, hrm = result.Item3 });
            }

            return NotFound();
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

            var fileName = $"Probation Report ({model.FromDate.Value.ToString("dd MMM yyyy")} - {model.ToDate.Value.ToString("dd MMM yyyy")}).xlsx";

            return File(fileContent, MimeType.XLSX, fileName);
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