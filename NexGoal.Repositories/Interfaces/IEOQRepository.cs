namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model;

    public interface IEOQRepository
    {
        IEnumerable<EOQNominatorSettingModel> GetEOQNominatorSettingById(ICriteria criteria);
    }
}