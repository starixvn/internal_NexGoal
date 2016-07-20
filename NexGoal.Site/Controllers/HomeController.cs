namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Authentication;
    using NexGoal.Model.Authentication;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private readonly IEmployeeService employeeService;

        public HomeController()
            : this(null)
        {
        }

        public HomeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
        }

        public ActionResult Index()
        {
            var criteria = new GetEmployeeByDomainUsernameCriteria()
            {
                DomainUsername = HttpContext.User.Identity.Name
            };
            UserSession user = employeeService.GetEmployeeByUsername(criteria);

            return View(user);
        }
    }
}