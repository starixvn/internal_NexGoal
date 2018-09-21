namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;

    public interface IPARecordService
    {
        Task<IEnumerable<PeriodModel>> GetPeriodByActorAsync(int actorId);

        Task<IEnumerable<PARecordModel>> GetPARecordByActorAsync(int actorId, int periodId);
    }
}