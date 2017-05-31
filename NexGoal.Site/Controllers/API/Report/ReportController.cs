namespace NexGoal.Site.Controllers.API
{
    using Core.Model;
    using Model.RoleManagement;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report.EPADetail;
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
        private readonly IRoleService roleService;
        private readonly IRecalculateScoreService recalculateScoreService;

        public ReportController()
            : this(null, null, null)
        {
        }

        public ReportController(
            IReportService reportService,
            IRoleService roleService,
            IRecalculateScoreService recalculateScoreService)
        {
            _reportService = reportService ?? new ReportService();
            this.recalculateScoreService = recalculateScoreService ?? new RecalculateScoreService();
            this.roleService = roleService ?? new RoleService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPADetailReport, Policy = Policy.Read)]
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

        [ApiAuthorize(FunctionKey = FunctionKey.MasterReport, Policy = Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetMasterReport(
            int periodId,
            string groupIds = null,
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

        public bool GetPermissionOnRecalculateScore()
        {
            IEnumerable<SystemRoleModel> roles = roleService.GetRolesByEmployeeId(SessionService.EmployeeId);
            IEnumerable<int> roleIds = roles.Select(x => x.RoleId);

            bool isHost = roleIds.Contains((int)SystemRole.SystemAdmin)
                || roleIds.Contains((int)SystemRole.HumanResource)
                || roleIds.Contains((int)SystemRole.Management);

            return isHost;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPADetailReport, Policy = Policy.Read)]
        [HttpPut]
        public HttpResponseMessage RecalculateScore(GetPAReportCriteria criteria)
        {
            HttpResponseMessage response;
            IEnumerable<SystemRoleModel> roles = roleService.GetRolesByEmployeeId(SessionService.EmployeeId);
            IEnumerable<int> roleIds = roles.Select(x => x.RoleId);

            if (roleIds.Contains((int)SystemRole.SystemAdmin)
                || roleIds.Contains((int)SystemRole.HumanResource)
                || roleIds.Contains((int)SystemRole.Management))
            {
                criteria.UserId = SessionService.EmployeeId;

                IEnumerable<EPADetailModel> epaInfos = _reportService.GetEPADetailReport(criteria);

                CommandResult commandResult = recalculateScoreService.RecalculateScore(SessionService.EmployeeId, epaInfos.Select(x => x.EPAId));

                response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            return response;
        }
    }
}