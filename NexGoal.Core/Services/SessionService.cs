namespace NexGoal.Core.Services
{
    using NexGoal.Core.Model;
    using System.Collections.Generic;
    using System.Web;

    public class SessionService : ISessionService
    {
        private readonly IAuthenticationService authenticationService;

        public SessionService()
            : this(null)
        {
        }

        public SessionService(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService ?? new AuthenticationService();
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
                if (session[Constants.Constants.CurrentSessionKey] == null)
                {
                    var domainUsername = HttpContext.Current.User.Identity.Name;
                    UserSession currentUser = authenticationService.GetEmployeeByUsername(domainUsername);
                    var username = domainUsername.Split('\\')[1];
                    authenticationService.InsertEmployeeIPAddress(username);
                    CurrentUser = currentUser;
                }

                return session[Constants.Constants.CurrentSessionKey] as UserSession;
            }

            set
            {
                HttpContext.Current.Session[Constants.Constants.CurrentSessionKey] = value;
            }
        }
    }
}
