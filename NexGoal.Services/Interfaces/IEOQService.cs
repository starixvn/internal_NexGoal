namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;

    public interface IEOQService
    {
        IEnumerable<EOQYModel> GetEOQ(GetEOQCriteria criteria);

        EOQYModel GetEOQById(GetEOQByIdCriteria criteria);

        IEnumerable<NominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria);

        CommandResult CreateEOQ(CreateEOQCriteria criteria);

        CommandResult UpdateEOQ(UpdateEOQCriteria criteria);

        CommandResult UpdateEOQNominatorSettingById(UpdateEOQNominatorSettingByIdCriteria criteria);

        CommandResult DeleteEOQ(DeleteEOQCriteria criteria);

        IEnumerable<int> GetYearEOQ();
    }
}