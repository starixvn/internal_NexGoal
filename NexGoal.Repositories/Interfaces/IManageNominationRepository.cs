namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using System.Collections.Generic;

    public interface IManageNominationRepository
    {
        IEnumerable<NominatorSettingModel> GetNominatorSettingById(ICriteria criteria);
    }
}