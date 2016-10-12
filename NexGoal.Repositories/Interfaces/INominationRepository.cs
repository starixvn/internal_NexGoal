namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using System.Collections.Generic;

    public interface INominationRepository
    {
        NominatorSettingModel GetLatestNominationByNominatorId(ICriteria criteria);

        IEnumerable<NominationModel> GetNominationByNominatorId(ICriteria criteria);

        NominationModel GetNominationById(ICriteria criteria);

        IEnumerable<NominationEPAModel> GetNominationByPeriodId(ICriteria criteria);
    }
}