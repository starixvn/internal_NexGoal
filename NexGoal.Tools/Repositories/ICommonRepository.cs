namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public interface ICommonRepository
    {
        IEnumerable<EPAModel> GetEPAInfoToChangeCertificate(IDbSetting setting, ICriteria criteria);

        IEnumerable<PAFocusModel> GetAllPAFocus(IDbSetting setting, ICriteria criteria);
    }
}