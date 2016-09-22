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
        private const string EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\Period\ExcelTemplates\EmployeeFlowSettingReport.xls";
        private const string MasterReportTemplatePath = @"~\ExcelTemplates\MasterReport.xls";

        private readonly IPeriodService periodService;
        private readonly IReportService reportService;
        private readonly IEPADetailReportService epaReportService;
        private readonly IMasterExportService masterExportService;
        private readonly ISessionService sessionService;

        public ExportReportController()
            : this(null, null, null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService,
            IEPADetailReportService epaReportService,
            IMasterExportService masterExportService,
            ISessionService sessionService)
        {
            this.periodService = checkFlowService ?? new PeriodService();
            this.reportService = reportService ?? new ReportService();
            this.epaReportService = epaReportService ?? new EPADetailReportService();
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
                var employeeFlowSettings = periodService.GetPeriodEmployeeSettingById(criteria);

                string templatePath = Server.MapPath(EmployeeFlowSettingReportTemplatePath);
                periodService.ExportEmployeeSettingReport(employeeFlowSettings, templatePath, criteria.PeriodName);
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPADetailReport(GetPAReportCriteria criteria)
        {
            criteria.UserId = this.sessionService.EmployeeId;

            IEnumerable<EPADetailModel> epaDetail = reportService.GetEPADetailReport(criteria);

            epaReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMasterReport(
            int periodId,
            int? groupIds = null,
            int? employeeId = null)
        {
            string templatePath = Server.MapPath(MasterReportTemplatePath);
            var criteria = new GetMasterReportCriteria()
            {
                EmployeeId = employeeId,
                PeriodId = periodId,
                GroupIds = groupIds
            };

            IEnumerable<MasterReportModel> employees = reportService.GetMaster(criteria);
            var period = this.periodService.GetPeriodById(periodId);
            this.masterExportService.Export(employees, templatePath, period.Name);

            return new EmptyResult();
        }
    }
}