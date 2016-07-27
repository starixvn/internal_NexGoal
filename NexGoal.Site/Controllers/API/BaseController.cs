namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Core.Services.Interfaces;
    using NexGoal.Criteria.Authentication;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web;
    using System.Web.Http;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;

        public BaseController()
            : this(null, null)
        {
        }

        public BaseController(ISessionService sessionService, IEmployeeService employeeService)
        {
            this.sessionService = sessionService ?? new SessionService();
            this.employeeService = employeeService ?? new EmployeeService();
        }

        public UserSession CurrentUser
        {
            get
            {
                var user = this.sessionService.CurrentUser;
                if (user == null)
                {
                    var criteria = new GetEmployeeByDomainUsernameCriteria()
                    {
                        DomainUsername = HttpContext.Current.User.Identity.Name
                    };
                    user = employeeService.GetEmployeeByUsername(criteria);

                    this.sessionService.CurrentUser = user;
                }

                return user;
            }
        }
    }
}