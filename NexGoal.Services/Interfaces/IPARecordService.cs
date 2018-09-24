namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Models.UserManagement;

    public interface IPARecordService
    {
        Task<IEnumerable<PeriodModel>> GetPeriodByActorAsync(int actorId);

        Task<IEnumerable<UserGroupModel>> GetGroupByActorAsync(int actorId, int periodId);

        Task<IEnumerable<PARecordModel>> GetPARecordByActorAsync(GetPARecordParam param);

        Task<PARecordModel> GetPARecordByIdAsync(int paRecordId, int actorId);

        Task<CommandResult> EditPARecordByReviewerAsync(int actorId, EditPARecordByReviewerModel paRecord);
    }
}