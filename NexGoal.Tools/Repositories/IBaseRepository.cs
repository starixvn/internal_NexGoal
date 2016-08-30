namespace NexGoal.Tools
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public interface IBaseRepository
    {
        CommandResult Insert(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false);
    }
}