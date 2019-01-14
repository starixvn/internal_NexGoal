namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.Core.Services;

    public class ReportController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPARecordService paRecordService;
        private readonly IExportPAReportService exportService;

        public ReportController(
            ISessionService sessionService,
            IPARecordService paRecordService,
            IExportPAReportService exportService)
        {
            this.sessionService = sessionService;
            this.paRecordService = paRecordService;
            this.exportService = exportService;
        }

        public IActionResult PAReport()
        {
            return View();
        }

        public async Task<JsonResult> GetPARecords(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            var paRecords = await paRecordService.GetPAReportAsync(model).ConfigureAwait(false);

            return Json(paRecords);
        }

        public async Task<IActionResult> ExportPAReport(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            var paRecords = await paRecordService.GetPAReportAsync(model).ConfigureAwait(false);
            var fileName = "PA Report";
            byte[] fileContent = exportService.ExportExcel(paRecords);

            return File(fileContent, MimeType.XLSX, fileName);
        }
    }
}