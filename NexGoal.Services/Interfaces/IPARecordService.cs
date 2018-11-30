namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public interface IPARecordService
    {
        Task<IEnumerable<PeriodBaseModel>> GetPeriodByActorAsync(int actorId);

        Task<IEnumerable<GroupBaseModel>> GetGroupByActorAsync(int actorId, int periodId);

        Task<IEnumerable<PARecordModel>> GetPARecordByReviewerAsync(GetPARecordParam param);

        Task<PARecordModel> GetPARecordByIdAsync(int paRecordId, int actorId);

        Task<CommandResult> EditPARecordByReviewerAsync(int actorId, EditPARecordByReviewerModel paRecord);

        Task<CommandResult> QuickEditPARecordByReviewerAsync(int actorId, int paRecordId);
    }
}