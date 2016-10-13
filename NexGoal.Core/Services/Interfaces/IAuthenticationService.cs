using NexGoal.Core.Model;

namespace NexGoal.Core.Services
{
    public interface IAuthenticationService
    {
        UserSession GetEmployeeByUsername(string domainName);

        void InsertEmployeeIPAddress(string nickName);
    }
}
