namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model.PAFocus;
    using NexGoal.Model.Period;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;

    public class ExportPAFocusReportController : Controller
    {
        private readonly IPAFocusReportService reportService;
        private readonly ISessionService sessionService;
        private readonly IPAFocusService focusService;

        public ExportPAFocusReportController()
            : this(null, null, null)
        {
        }

        public ExportPAFocusReportController(
            IPAFocusReportService reportService,
            ISessionService sessionService,
            IPAFocusService focusService)
        {
            this.reportService = reportService ?? new PAFocusReportService();
            this.sessionService = sessionService ?? new SessionService();
            this.focusService = focusService ?? new PAFocusService();
        }

        public ActionResult ExportPAFocus(int periodId, string teamIds = null)
        {
            string fileName = "PAFocusReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            var criteria = new GetPAFocusCriteria
            {
                PeriodId = periodId,
                GroupIds = teamIds,
                EvaluatorId = this.sessionService.EmployeeId
            };
            IEnumerable<PAFocusModel> focuses = focusService.GetPAFocus(criteria);

            MemoryStream output = reportService.ExportPAFocus(focuses, periodId);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}