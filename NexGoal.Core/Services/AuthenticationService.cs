namespace NexGoal.Core.Services
{
    using NexGoal.Core.Criteria;
    using NexGoal.Core.Model;
    using NexGoal.Core.Repositories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;

    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly IIAuthenticationRepository _authenticationRepository;

        public AuthenticationService()
            : this(null)
        {
        }

        public AuthenticationService(IIAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository ?? new AuthenticationRepository();
        }

        public UserSession GetEmployeeByUsername(string domainName)
        {
            var criteria = new GetAuthenticationByDomainUsernameCriteria { DomainUsername = domainName };
            var employee = new UserSession();

            if (criteria != null && criteria.IsValid())
            {
                employee = _authenticationRepository.GetEmployee(criteria);
            }

            return employee;
        }

        public void InsertLogLogin(string username)
        {
            var criteria = new InsertLogLoginCriteria() 
            {
                IdentifyUser = GetShortUsername(HttpContext.Current.User.Identity.Name),
                LoginUser = GetShortUsername(username),
                MachineName = DetermineCompName(HttpContext.Current.Request.UserHostName),
                IPAddress = HttpContext.Current.Request.UserHostAddress,
                BrowserName = HttpContext.Current.Request.Headers["User-Agent"]
            };

            Repository.Insert(criteria);
        }

        public string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }

        private string GetShortUsername(string username)
        {
            if(username.IndexOf('\\') > 0)
            {
                username = username.Split('\\')[1];
            }

            return username;
        }
    }
}
