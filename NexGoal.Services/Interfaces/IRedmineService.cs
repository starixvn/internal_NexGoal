namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Redmine;

    public interface IRedmineService
    {
        Task<IEnumerable<TeamWorkloadModel>> GetTeamWorkload(int EmployeeId, int periodId);

        Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadAsync(int employeeId, int periodId);

        Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadForPMAsync(int employeeId);

        Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadForLeaderAsync(int employeeId);
    }
}