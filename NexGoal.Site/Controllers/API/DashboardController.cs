namespace NexGoal.Site.Controllers.API
{
    using System.Linq;
    using System.Web.Http;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.ApiAuthorizeAttribute;

    public class DashboardController : BaseController
    {
        private readonly IReportService _reportService;

        public DashboardController() :
            this(null)
        {
        }

        public DashboardController(
            IReportService reportService)
        {
            _reportService = reportService ?? new ReportService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = new DashboardModel();

            var criteria = new GetDashboardCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            dashboard = _reportService.GetDashboard(criteria);
            dashboard.Employee = CurrentUser.EmployeeInfo;
            dashboard.MonthlyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId == (int)TargetType.Monthly);
            dashboard.MyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId != (int)TargetType.Monthly);

            return dashboard;
        }
    }
}