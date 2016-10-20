namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using Fanex.Data;

    public interface ICommonService
    {
        CommandResult ResetScore(IDbSetting setting, ResetScoreCriteria criteria);

        IEnumerable<EPAModel> GetEPAInfo(string connectionString, int idFrom, int idTo);

        CommandResult UpdateEPAScore(string connectionString, IEnumerable<EPAModel> epas);

        IEnumerable<PAFocusModel> GetAllPAFocus(string connectionString, int idFrom, int idTo);

        CommandResult UpdatePAFocus(string connectionString, IEnumerable<PAFocusModel> evaluations);
    }
}