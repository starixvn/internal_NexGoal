namespace NexGoal.Tools
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public interface ICommonRepository
    {
        EPAViewModel GetEPAInfoToChangeCertificate(IDbSetting setting, ICriteria criteria);
    }
}