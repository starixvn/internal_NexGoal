namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Authentication;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web;
    using System.Web.Http;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        private static IEmployeeService employeeService = new EmployeeService();

        public static UserSession CurrentUser
        {
            get
            {
                UserSession currentUser = null;
                var session = HttpContext.Current.Session;
                if (session[Constants.CurrentSessionKey] != null)
                {
                    currentUser = (UserSession)session[Constants.CurrentSessionKey];
                }
                else
                {
                    var criteria = new GetEmployeeByDomainUsernameCriteria()
                    {
                        DomainUsername = HttpContext.Current.User.Identity.Name
                    };
                    currentUser = employeeService.GetEmployeeByUsername(criteria);

                    if (currentUser != null)
                    {
                        HttpContext.Current.Session[Constants.CurrentSessionKey] = currentUser;
                    }
                }

                return currentUser;
            }
        }
    }
}