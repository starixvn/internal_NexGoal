using NexGoal.Core.Criteria;
using NexGoal.Core.Model;
using NexGoal.Core.Repositories;

namespace NexGoal.Core.Services
{
    public class AuthenticationService : IAuthenticationService
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
    }
}
