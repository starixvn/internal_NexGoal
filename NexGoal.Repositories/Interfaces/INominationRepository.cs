namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model;

    public interface INominationRepository
    {
        NominatorSettingModel GetLatestNominationByNominatorId(ICriteria criteria);

        IEnumerable<NominationModel> GetNominationByNominatorId(ICriteria criteria);

        NominationModel GetNominationById(ICriteria criteria);
    }
}