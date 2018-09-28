namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models;
    using NexGoal.Models.PARecord;

    public interface IDashboardService
    {
        Task<IEnumerable<PARecordModel>> GetPARecordAsync();

        Task<IEnumerable<UserWorkLoadModel>> GetWorkLoad(int employeeId);
    }
}