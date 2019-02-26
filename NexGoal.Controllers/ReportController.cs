namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
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
            IExportPAReportService exportService
            )
        {
            this.sessionService = sessionService;
            this.reportService = reportService;
            this.exportService = exportService;
        }

        public IActionResult PAReport()
        {
            return View();
        }

        public async Task<JsonResult> GetGroupAndMember()
        {
            Tuple<IEnumerable<Group>, IEnumerable<Employee>> tuple = await reportService.GetGroupAndMember(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(new { Groups = tuple.Item1, Members = tuple.Item2 });
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
            byte[] fileContent = await exportService.ExportExcel(model).ConfigureAwait(false);
            var fileName = "PA Report";

            return File(fileContent, MimeType.XLSX, fileName);
        }

        public async Task<IActionResult> PaDetail(int paRecordId)
        {
            var tuple = await exportService.ExportPADetailPDF(sessionService.EmployeeId, paRecordId).ConfigureAwait(false);
            return File(tuple.Item2, MimeType.PDF, tuple.Item1);
        }
    }
}