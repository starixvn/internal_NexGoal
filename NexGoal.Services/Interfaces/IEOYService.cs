namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using System.Collections.Generic;

    public interface IEOYService
    {
        IEnumerable<EOQYModel> GetEOY(GetEOYCriteria criteria);

        EOQYModel GetEOYById(GetEOYByIdCriteria criteria);

        IEnumerable<NominatorSettingModel> GetEOYNominatorSettingById(GetEOYNominatorSettingByIdCriteria criteria);

        CommandResult CreateEOY(CreateEOYCriteria criteria);

        CommandResult UpdateEOY(UpdateEOYCriteria criteria);

        CommandResult UpdateEOYNominatorSettingById(UpdateEOYNominatorSettingByIdCriteria criteria);

        CommandResult DeleteEOY(DeleteEOYCriteria criteria);
    }
}