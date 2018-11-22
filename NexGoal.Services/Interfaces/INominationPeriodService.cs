namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.NominationPeriod;

    public interface INominationPeriodService
    {
        Task<IEnumerable<NominationPeriodModel>> GetAsync(bool? isCompleted = null);
    }
}