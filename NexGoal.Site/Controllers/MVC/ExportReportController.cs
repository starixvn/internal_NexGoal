namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.AttendanceSummary;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.AttendanceSummary;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using NexGoal.Resources;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;

    public class ExportReportController : Controller
    {
        private const string _EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\Period\ExcelTemplates\EmployeeFlowSettingReport.xls";

        private readonly IPeriodService _periodService;
        private readonly IReportService _reportService;
        private readonly IEPADetailReportService _epaDetailReportService;
        private readonly ISessionService sessionService;

        public ExportReportController()
            : this(null, null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService,
            IEPADetailReportService epaDetailReportService,
            ISessionService sessionService)
        {
            _periodService = checkFlowService ?? new PeriodService();
            _reportService = reportService ?? new ReportService();
            _epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
            this.sessionService = sessionService ?? new SessionService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportEmployeeSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            if (criteria != null)
            {
                var employeeFlowSettings = _periodService.GetPeriodEmployeeSettingById(criteria);

                string templatePath = Server.MapPath(_EmployeeFlowSettingReportTemplatePath);
                _periodService.ExportEmployeeSettingReport(employeeFlowSettings, templatePath, criteria.PeriodName);
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPADetailReport(GetPAReportCriteria criteria)
        {
            criteria.UserId = this.sessionService.EmployeeId;

            IEnumerable<EPADetailModel> epaDetail = _reportService.GetEPADetailReport(criteria);

            _epaDetailReportService.Render(epaDetail);

            return new EmptyResult();
        }
    }
}