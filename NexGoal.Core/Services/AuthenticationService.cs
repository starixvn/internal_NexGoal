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

        public void InsertEmployeeIPAddress(string username)
        {
            var criteria = new InsertLogLoginCriteria() 
            {
                WindowsUser = Environment.UserName,
                IdentifyUser = HttpContext.Current.User.Identity.Name,
                LoginUser = username,
                IPAddress = GetLocalIPAddress()
            };

            Repository.Insert(criteria);
        }

        public static string GetLocalIPAddress()
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
