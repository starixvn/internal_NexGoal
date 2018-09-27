namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.PARecord;

    public interface IDashboardService
    {
        Task<IEnumerable<PARecordModel>> GetPARecordAsync();
    }
}