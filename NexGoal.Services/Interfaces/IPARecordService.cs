namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Models.UserManagement;

    public interface IPARecordService
    {
        Task<IEnumerable<PeriodModel>> GetPeriodByActorAsync(int actorId);

        Task<IEnumerable<UserGroupModel>> GetGroupByActorAsync(int actorId, int periodId);

        Task<IEnumerable<PARecordModel>> GetPARecordByActorAsync(int actorId, int periodId);
    }
}