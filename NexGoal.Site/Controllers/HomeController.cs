namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Core.Services.Interfaces;
    using NexGoal.Criteria.Authentication;
    using NexGoal.Model.Authentication;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly ISessionService sessionService;

        public HomeController()
            : this(null, null)
        {
        }

        public HomeController(IEmployeeService employeeService, ISessionService sessionService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.sessionService = sessionService ?? new SessionService();
        }

        public ActionResult Index()
        {
            var criteria = new GetEmployeeByDomainUsernameCriteria()
            {
                DomainUsername = HttpContext.User.Identity.Name
            };
            UserSession user = employeeService.GetEmployeeByUsername(criteria);
            this.sessionService.CurrentUser = user;

            return View();
        }
    }
}