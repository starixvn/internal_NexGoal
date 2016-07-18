namespace NexGoal.Site.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Criteria.AttendanceSummary;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.AttendanceSummary;
    using NexGoal.Model.Period;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using NexGoal.Resources;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;

    public class ExportReportController : Controller
    {
        private const string _EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\Period\ExcelTemplates\EmployeeFlowSettingReport.xls";
        private const string _DisciplineSummaryTemplatePath = @"~\SourceApp\Components\AttendanceSummary\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string _DisciplineDetailTemplatePath = @"~\SourceApp\Components\AttendanceSummary\ExcelTemplates\DisciplineDetailReport.xls";

        private readonly IPeriodService _periodService;
        private readonly IReportService _reportService;
        private readonly IAttendanceSummaryService _attendanceService;
        private readonly IEPADetailReportService _epaDetailReportService;
        private readonly IMonthlyEPADetailReportService _monthlyEPADetailReportService;

        public ExportReportController()
            : this(null, null, null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService,
            IAttendanceSummaryService attendanceService,
            IEPADetailReportService epaDetailReportService,
            IMonthlyEPADetailReportService monthlyEPADetailReportService)
        {
            _periodService = checkFlowService ?? new PeriodService();
            _reportService = reportService ?? new ReportService();
            _attendanceService = attendanceService ?? new AttendanceSummaryService();
            _epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
            _monthlyEPADetailReportService = monthlyEPADetailReportService ?? new MonthlyEPADetailReportService();
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

        public ActionResult ExportEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            string fileName = Lang.EPASummaryReportName + ".xls";

            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetEPASummaryReport(criteria);

            MemoryStream output = _reportService.ExportEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            string fileName = Lang.MonthlyEPASummaryReportName + ".xls";

            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetMonthlyEPASummaryReport(criteria);

            MemoryStream output = _reportService.ExportMonthlyEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportEPADetailReport(GetEPADetailCriteria criteria)
        {
            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetEPADetailReport(criteria);

            _epaDetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetMonthlyEPADetailReport(criteria);
            _monthlyEPADetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria)
        {
            criteria.ApproverId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = _attendanceService.GetAttendanceSummary(criteria);

            if (attendanceEmployees != null)
            {
                string templatePath = Server.MapPath(_DisciplineSummaryTemplatePath);
                _attendanceService.ExportAttendanceSummaryReport(criteria, attendanceEmployees, templatePath);
            }

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ApproverId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
                IEnumerable<AttendanceDetailModel> attendanceViolations = _attendanceService.GetAttendanceDetail(criteria);

                if (attendanceViolations != null)
                {
                    string templatePath = Server.MapPath(_DisciplineDetailTemplatePath);
                    _attendanceService.ExportAttendanceDetailReport(criteria, attendanceViolations, templatePath);
                }
            }

            return new EmptyResult();
        }
    }
}