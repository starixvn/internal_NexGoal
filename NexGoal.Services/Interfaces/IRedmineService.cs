namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Redmine;

    public interface IRedmineService
    {
        Task<IEnumerable<TeamWorkloadModel>> GetTeamWorkload(int EmployeeId, int periodId);

        Task<IEnumerable<ProjectWorkloadModel>> GetWorkloadAsync(int employeeId, int periodId);
    }
}