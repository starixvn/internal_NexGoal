namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;

    [NexGoalPermission(FunctionKey = FunctionKey.MasterReport, Policy = Policy.Read)]
    public class MasterReportController : Controller
    {
        private readonly IMasterReportService masterReportService;

        public MasterReportController(IMasterReportService masterReportService)
        {
            this.masterReportService = masterReportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriods()
        {
            var result = await masterReportService.GetPeriods().ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecords(int periodId, string groupIds, string employeeIds)
        {
            var result = await masterReportService.GetRecords(periodId, groupIds, employeeIds).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Export(int periodId, string groupIds, string employeeIds)
        {
            var fileContent = await masterReportService.ExportAsync(periodId, groupIds, employeeIds).ConfigureAwait(false);

            return File(fileContent, MimeType.XLSX, "Master Report.xlsx");
        }
    }
}