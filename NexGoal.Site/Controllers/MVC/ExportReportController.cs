namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.AttendanceSummary;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.AttendanceSummary;
    using NexGoal.Model.Report;
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
        private const string _MasterReportTemplatePath = @"~\ExcelTemplates\MasterReport.xls";

        private readonly IPeriodService _periodService;
        private readonly IReportService _reportService;
        private readonly IEPADetailReportService _paReportService;
        private readonly IMasterExportService masterExportService;
        private readonly ISessionService sessionService;

        public ExportReportController()
            : this(null, null, null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService,
            IEPADetailReportService paReportService,
            IMasterExportService masterExportService,
            ISessionService sessionService)
        {
            _periodService = checkFlowService ?? new PeriodService();
            _reportService = reportService ?? new ReportService();
            _paReportService = paReportService ?? new EPADetailReportService();
            this.sessionService = sessionService ?? new SessionService();
            this.masterExportService = masterExportService ?? new MasterExportService();
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

            _paReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMasterReport(
            int periodId,
            int? groupIds = null,
            int? employeeId = null)
        {
            string templatePath = Server.MapPath(_MasterReportTemplatePath);
            var criteria = new GetMasterReportCriteria()
            {
                EmployeeId = employeeId,
                PeriodId = periodId,
                GroupIds = groupIds
            };

            IEnumerable<MasterReportModel> employees = _reportService.GetMaster(criteria);
            var period= this._periodService.GetPeriodById(periodId);
            this.masterExportService.Export(employees,templatePath,period.Name);
            return new EmptyResult();
        }
    }
}