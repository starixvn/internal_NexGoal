namespace NexGoal.Tools
{
    using Fanex.Data;

    public interface ICommonService
    {
        CommandResult ResetScore(IDbSetting setting, ResetScoreCriteria criteria);

        EPAViewModel GetEPAInfoToChangeCertificate(IDbSetting setting, GetEPAInfoToChangeCertificateCriteria criteria);

        CommandResult UpdateEPAScore(IDbSetting setting, UpdateEPAScoreCriteria criteria);
    }
}