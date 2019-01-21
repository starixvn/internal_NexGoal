namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;
    using NexGoal.Services;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.Core.Services;

    public class ReportController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPAReportService reportService;
        private readonly IExportPAReportService exportService;

        public ReportController(
            ISessionService sessionService,
            IPAReportService reportService,
            IExportPAReportService exportService)
        {
            this.sessionService = sessionService;
            this.reportService = reportService;
            this.exportService = exportService;
        }

        public IActionResult PAReport()
        {
            return View();
        }

        public async Task<JsonResult> GetPARecords(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            var paRecords = await reportService.GetPAReportAsync(model).ConfigureAwait(false);

            return Json(paRecords);
        }

        public async Task<IActionResult> ExportPAReport(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            var paRecords = await reportService.GetPAReportAsync(model).ConfigureAwait(false);
            var fileName = "PA Report";
            byte[] fileContent = exportService.ExportExcel(paRecords);

            return File(fileContent, MimeType.XLSX, fileName);
        }

        [AllowAnonymous]
        public async Task<IActionResult> PaDetail(GetPADetailParam model)
        {
            var reportDetail = await reportService.GetPAReportDetailAsync(model).ConfigureAwait(false);

            ViewData["NoLayout"] = true;
            return View(reportDetail);
        }
    }
}