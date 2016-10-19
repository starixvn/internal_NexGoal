namespace NexGoal.Core.Services
{
    using Fanex.Logging;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Criteria;
    using NexGoal.Core.Model;
    using NexGoal.Core.Repositories;
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
                IdentifyUser = GetShortUsername(HttpContext.Current.User.Identity.Name),
                LoginUser = GetShortUsername(username),
                MachineName = GetHostName(HttpContext.Current.Request.UserHostName),
                IPAddress = HttpContext.Current.Request.UserHostAddress,
                BrowserName = HttpContext.Current.Request.Headers["User-Agent"]
            };

            Repository.Insert(criteria);
        }

        public string GetHostName(string ipAddress)
        {
            string hostName = string.Empty;
            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                IPHostEntry entry = Dns.GetHostEntry(ip);
                if (entry != null)
                {
                    hostName = entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != Constants.NotFoundHostName)
                {
                    LogService.Error("Can not find Host Name", ex);
                }                
            }

            return hostName;
        }

        private string GetShortUsername(string username)
        {
            if (username.IndexOf('\\') > 0)
            {
                username = username.Split('\\')[1];
            }

            return username;
        }
    }
}
