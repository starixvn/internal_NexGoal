namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Period;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly CommonController _commonController;

        public ReportController()
            : this(null, null)
        {
        }

        public ReportController(
            IReportService reportService,
            CommonController commonController)
        {
            _reportService = reportService ?? new ReportService();
            _commonController = commonController ?? new CommonController();
        }

        [ApiAuthorize(FunctionKey.EPADetailReport, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPADetailReport(
            string periodIds = null,
            string groupIds = null,
            int? employeeId = null)
        {
            var criteria = new GetPAReportCriteria()
            {
                UserId = SessionService.EmployeeId,
                PeriodIds = periodIds,
                GroupIds = groupIds,
                EmployeeId = employeeId
            };

            IEnumerable<EPADetailModel> epaInfos = _reportService.GetEPADetailReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey.MasterReport, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetMasterReport(
            int periodId,
            int? groupIds = null,
            int? employeeId = null)
        {
            var criteria = new GetMasterReportCriteria()
            {
                EmployeeId = employeeId,
                PeriodId = periodId,
                GroupIds = groupIds
            };

            var report = _reportService.GetMaster(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, report);
        }
    }
}