namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Auth;
    using NexGoal.Models.Common;

    public interface IAuthService
    {
        Task<EmployeeSession> GetEmployeeByDomainUsernameAsync(string domainUsername);

        Task<IEnumerable<FunctionSession>> GetFunctionByEmployeeIdAsync(int employeeId);

        Task<IEnumerable<MenuItemSession>> GetMenuByEmployeeIdAsync(int employeeId);

        Task<Tuple<Employee, bool, int>> GetEmployeeInfoAsync(int employeeId);

        Task WriteLogLoginAsync(LogLogin logInfo);
    }
}