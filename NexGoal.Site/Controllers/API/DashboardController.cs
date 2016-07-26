namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Linq;
    using System.Web.Http;

    public class DashboardController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IEmployeeService employeeService;

        public DashboardController() :
            this(null, null)
        {
        }

        public DashboardController(
            IReportService reportService,
            IEmployeeService employeeService)
        {
            _reportService = reportService ?? new ReportService();
            this.employeeService = employeeService ?? new EmployeeService();
        }

        [HttpGet]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = new DashboardModel();

            var criteria = new GetDashboardCriteria()
            {
                EmployeeId = CurrentUser.EmployeeId
            };

            dashboard = _reportService.GetDashboard(criteria);
            dashboard.Employee = this.employeeService.GetEmployeeById(CurrentUser.EmployeeId);
            dashboard.MonthlyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId == (int)TargetType.Monthly);
            dashboard.MyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId != (int)TargetType.Monthly);

            return dashboard;
        }
    }
}