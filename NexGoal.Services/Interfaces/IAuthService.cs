namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Auth;

    public interface IAuthService
    {
        Task<EmployeeSession> GetEmployeeByDomainUsernameAsync(string domainUsername);

        Task<IEnumerable<FunctionSession>> GetFunctionByEmployeeIdAsync(int employeeId);

        Task<IEnumerable<MenuItemSession>> GetMenuByEmployeeIdAsync(int employeeId);
    }
}