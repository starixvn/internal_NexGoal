namespace NexGoal.Core.Services
{
    using NexGoal.Core.Criteria;
    using NexGoal.Core.Model;
    using NexGoal.Core.Repositories;
    using System;
    using System.Net;
    using System.Net.Sockets;
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
                WindowsUser = Environment.UserName,
                IdentifyUser = GetShortUsername(HttpContext.Current.User.Identity.Name),
                LoginUser = GetShortUsername(username),
                IPAddress = GetLocalIPAddress()
            };

            Repository.Insert(criteria);
        }

        private string GetShortUsername(string username)
        {
            if(username.IndexOf('\\') > 0)
            {
                username = username.Split('\\')[1];
            }

            return username;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }
    }
}
