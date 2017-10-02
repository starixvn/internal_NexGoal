namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Dashboard;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Linq;
    using System.Web.Http;

    public class DashboardController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IDashboardService dashboardService;

        public DashboardController() :
            this(null, null)
        {
        }

        public DashboardController(
            IReportService reportService,
            IDashboardService dashboardService)
        {
            this.reportService = reportService ?? new ReportService();
            this.dashboardService = dashboardService ?? new DashboardService();
        }

        [HttpGet]
        public DashboardModel GetDashboard()
        {
            var criteria = new GetDashboardCriteria()
            {
                EmployeeId = SessionService.EmployeeId
            };

            DashboardModel dashboard = reportService.GetDashboard(criteria);
            dashboard.MyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId != (int)TargetType.Monthly);
            dashboard.ToDoList = dashboardService.GetToDoList(SessionService.EmployeeId);

            return dashboard;
        }
    }
}