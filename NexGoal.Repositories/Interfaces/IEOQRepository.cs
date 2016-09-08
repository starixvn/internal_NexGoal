namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model;

    public interface IEOQRepository
    {
        IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(ICriteria criteria);

        EOQYNominatorSettingModel GetCurrentEOQByNominatorId(ICriteria criteria);

        IEnumerable<NominationModel> GetNominationByNominatorId(ICriteria criteria);
    }
}