namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MasterReport, Policy = Policy.Read)]
    public class MasterReportController : BaseController
    {
        private readonly IMasterReportService masterReportService;

        public MasterReportController(
            IMasterReportService masterReportService,
            ISessionService sessionService) : base(sessionService)
        {
            this.masterReportService = masterReportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Export(int periodId, string groupIds, string employeeIds)
        {
            var fileContent = await masterReportService.ExportAsync(periodId, groupIds, employeeIds, sessionService.EmployeeId).ConfigureAwait(false);

            return File(fileContent, MimeType.XLSX, "Master Report.xlsx");
        }
    }
}