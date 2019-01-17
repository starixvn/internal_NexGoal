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
        private readonly IPAReportService reportService;
        private readonly IExportPAReportService exportService;

        public ReportController(
            ISessionService sessionService,
            IPARecordService paRecordService,
            IPAReportService reportService,
            IExportPAReportService exportService)
        {
            this.sessionService = sessionService;
            this.paRecordService = paRecordService;
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

        public async Task<IActionResult> ExportPADetail(int paRecordId)
        {
            var record = await paRecordService.GetPARecordByIdAsync(paRecordId, sessionService.EmployeeId).ConfigureAwait(false);
            byte[] fileContent = exportService.ExportPADetail(record);
            var fileName = string.Format("{0}'s_PA_on_{1}", record.Employee.NickName, record.Period.Name);
            return File(fileContent, MimeType.PDF, fileName);
        }
    }
}