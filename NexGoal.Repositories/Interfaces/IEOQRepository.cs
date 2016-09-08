namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model;

    public interface IEOQRepository
    {
        IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(ICriteria criteria);

        EOQYNominatorSettingModel GetCurrentEOQByNominatorId(ICriteria criteria);

        IEnumerable<EOQYNominationModel> GetNominationByNominatorId(ICriteria criteria);

        EOQYNominationModel GetEOQNominationById(ICriteria criteria);
    }
}