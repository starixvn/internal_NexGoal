using NexGoal.Core.Constants;
using NexGoal.Core.Model;
using NexGoal.Core.Services.Interfaces;
using NexGoal.Criteria.Authentication;
using NexGoal.Services.Interfaces;
using System.Collections.Generic;
using System.Web;

namespace NexGoal.Services.Implement
{
    public class SessionService : ISessionService
    {
        private readonly IEmployeeService employeeService;

        public SessionService()
            : this(null)
        {
        }

        public SessionService(IEmployeeService employeeService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
        }

        public int EmployeeId
        {
            get
            {
                return CurrentUser.EmployeeId;
            }

            set
            {
                CurrentUser.EmployeeId = value;
            }
        }

        public IEnumerable<FunctionModel> Functions
        {
            get
            {
                return CurrentUser.Functions;
            }

            set
            {
                CurrentUser.Functions = value;
            }
        }

        public UserSession CurrentUser
        {
            get
            {
                var session = HttpContext.Current.Session;
                if (session[Constants.CurrentSessionKey] == null)
                {
                    var criteria = new GetEmployeeByDomainUsernameCriteria()
                    {
                        DomainUsername = HttpContext.Current.User.Identity.Name
                    };
                    UserSession currentUser = employeeService.GetEmployeeByUsername(criteria);
                    CurrentUser = currentUser;
                }

                return session[Constants.CurrentSessionKey] as UserSession;
            }

            set
            {
                HttpContext.Current.Session[Constants.CurrentSessionKey] = value;
            }
        }
    }
}
