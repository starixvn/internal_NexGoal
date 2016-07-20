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
        private static UserSession currentUser;
        private static int currentUserId;

        private static IEmployeeService employeeService = new EmployeeService();

        public static int CurrentUserId
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var session = HttpContext.Current.Session;
                    if (session != null && session[Constants.CurrentSessionUserId] != null)
                    {
                        currentUserId = (int)session[Constants.CurrentSessionUserId];
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
                            currentUserId = currentUser.EmployeeInfo.EmployeeId;
                            HttpContext.Current.Session[Constants.CurrentSessionUserId] = currentUserId;
                        }
                    }
                }

                return currentUserId;
            }
        }

        public static UserSession CurrentUser
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var session = HttpContext.Current.Session;
                    if (session != null && session[Constants.CurrentSessionKey] != null)
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
                }

                return currentUser;
            }
        }

        public static void ClearAll()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Response.Cookies.Clear();
            }
        }
    }
}