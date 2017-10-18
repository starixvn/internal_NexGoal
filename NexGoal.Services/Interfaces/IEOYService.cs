namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IEOYService
    {
        IEnumerable<EOQYModel> GetEOY(GetEOYCriteria criteria);

        EOQYModel GetEOYById(GetEOYByIdCriteria criteria);

        IEnumerable<NominatorSettingModel> GetEOYNominatorSettingById(GetEOYNominatorSettingByIdCriteria criteria);
    }
}