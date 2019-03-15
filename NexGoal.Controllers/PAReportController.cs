namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.PAReport, Policy = Policy.Read)]
    public class PAReportController : BaseController
    {
        private readonly IPAReportService reportService;
        private readonly IExportPAReportService exportService;
        private readonly IPARecordService paService;

        public PAReportController(
            ISessionService sessionService,
            IPAReportService reportService,
            IExportPAReportService exportService,
            IPARecordService paService) : base(sessionService)
        {
            this.reportService = reportService;
            this.exportService = exportService;
            this.paService = paService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetGroupAndMember()
        {
            Tuple<IEnumerable<Group>, IEnumerable<Employee>> tuple = await reportService.GetGroupAndMemberAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(new { Groups = tuple.Item1, Members = tuple.Item2 });
        }

        public async Task<JsonResult> GetPARecords(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;

            var paRecords = await reportService.GetPAReportAsync(model).ConfigureAwait(false);

            return Json(paRecords);
        }

        public async Task<IActionResult> ExportExcel(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            byte[] fileContent = await exportService.ExportExcelAsync(model).ConfigureAwait(false);
            const string fileName = "PA Report";

            return File(fileContent, MimeType.XLSX, fileName);
        }

        public async Task<IActionResult> ExportPdf(int id)
        {
            Policy permission = await paService.GetPermissionOnPerformanceAppraisalAsync(sessionService.EmployeeId, id).ConfigureAwait(false);

            if (permission == Policy.None)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var paDetails = await reportService.GetPAReportDetailAsync(sessionService.EmployeeId, id).ConfigureAwait(false);

            if (paDetails.PARecord != null)
            {
                var tuple = await exportService.ExportPdfAsync(paDetails).ConfigureAwait(false);
                return File(tuple.Item2, MimeType.PDF, tuple.Item1);
            }

            return RedirectToAction("PageNotFound", "Error");
        }
    }
}