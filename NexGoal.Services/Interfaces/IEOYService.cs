namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;

    public interface IEOYService
    {
        IEnumerable<EOQYModel> GetEOY(GetEOYCriteria criteria);

        EOQYModel GetEOYById(GetEOYByIdCriteria criteria);

        IEnumerable<EOQYNominatorSettingModel> GetEOYNominatorSettingById(GetEOYNominatorSettingByIdCriteria criteria);

        EOQYNominatorSettingModel GetCurrentEOYByNominatorId(int nominatorId);

        CommandResult CreateEOY(CreateEOYCriteria criteria);

        CommandResult UpdateEOY(UpdateEOYCriteria criteria);

        CommandResult UpdateEOYNominatorSettingById(UpdateEOYNominatorSettingByIdCriteria criteria);

        CommandResult DeleteEOY(DeleteEOYCriteria criteria);
    }
}