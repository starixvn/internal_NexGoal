namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using jsreport.AspNetCore;
    using jsreport.Types;
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
            model.EmployeeIds = string.IsNullOrEmpty(model.EmployeeIds) ? sessionService.EmployeeId.ToString() :
              string.Join($",{sessionService.EmployeeId}", model.EmployeeIds);

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

        [MiddlewareFilter(typeof(JsReportPipeline))]
        public IActionResult PaDetail(GetPADetailParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            var contentDisposition = $"attachment; filename=\"Oliver_PAReport.pdf\"";

            HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf)
                .Configure((r) => r.Template.Chrome = new Chrome
                {
                    MarginLeft = ".5cm",
                    MarginRight = ".5cm",
                    MarginTop = "1cm",
                    MarginBottom = "1cm",
                    PrintBackground = true
                }).OnAfterRender((r) => HttpContext.Response.Headers["Content-Disposition"] = contentDisposition);

            return View();
        }
    }
}