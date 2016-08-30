namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using Fanex.Data;

    public interface ICommonService
    {
        CommandResult ResetScore(IDbSetting setting, ResetScoreCriteria criteria);

        IList<EPAModel> GetEPAInfoToChangeCertificate(GetEPAInfoToChangeCertificateCriteria criteria);
    }
}