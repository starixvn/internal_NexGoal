namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using Fanex.Data;

    public interface ICommonService
    {
        CommandResult ResetScore(IDbSetting setting, ResetScoreCriteria criteria);

        IList<EPAModel> GetEPAInfoToChangeCertificate(IDbSetting setting, GetEPAInfoToChangeCertificateCriteria criteria);

        CommandResult UpdateEPAScore(IDbSetting setting, UpdateEPAScoreCriteria criteria);

        IEnumerable<PAFocusModel> GetAllPAFocus(string connectionString);

        CommandResult UpdatePAFocus(string connectionString, IEnumerable<PAFocusModel> evaluations);
    }
}