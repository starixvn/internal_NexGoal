namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IEOQService
    {
        IEnumerable<EOQYModel> GetEOQ(GetEOQCriteria criteria);

        EOQYModel GetEOQById(GetEOQByIdCriteria criteria);

        IEnumerable<NominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria);

        IEnumerable<int> GetYearEOQ();
    }
}