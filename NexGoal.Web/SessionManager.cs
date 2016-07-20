namespace NexGoal.Web
{
    using NexGoal.Core.Constants;
    using NexGoal.Criteria.Authentication;
    using NexGoal.Model.Authentication;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web;

    public static class SessionManager
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