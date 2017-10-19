namespace NexGoal.Site.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class ExportReportController : Controller
    {
        private const string EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\Period\ExcelTemplates\EmployeeFlowSettingReport.xls";

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
            IPeriodService periodService,
            IReportService reportService,
            IEPADetailReportService epaReportService,
            IMasterExportService masterExportService,
            ISessionService sessionService)
        {
            this.periodService = periodService ?? new PeriodService();
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
            if (criteria != null)
            {
                criteria.UserId = sessionService.EmployeeId;
                IEnumerable<EPADetailModel> epaDetail = reportService.GetEPADetailReport(criteria);

                epaReportService.Render(epaDetail);
            }

            return new EmptyResult();
        }

        public ActionResult ExportMasterReport(
            int periodId,
            string groupIds = null,
            int? employeeId = null)
        {
            var criteria = new GetMasterReportCriteria
            {
                EmployeeId = employeeId,
                PeriodId = periodId,
                GroupIds = groupIds
            };

            IEnumerable<MasterReportModel> employees = reportService.GetMaster(criteria);
            var period = periodService.GetPeriodById(periodId);
            MemoryStream output = masterExportService.Export(employees, period.Name);

            return File(output.ToArray(), "application/vnd.ms-excel", "MasterReport_" + period.Name + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }
    }
}