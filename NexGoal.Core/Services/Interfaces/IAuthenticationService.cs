namespace NexGoal.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IAuthenticationService
    {
        Task<EmployeeSession> GetEmployeeByDomainUsernameAsync(string domainUsername);

        Task<IEnumerable<FunctionSession>> GetFunctionByEmployeeIdAsync(int employeeId);

        Task<IEnumerable<MenuItemSession>> GetMenuByEmployeeIdAsync(int employeeId);
    }
}