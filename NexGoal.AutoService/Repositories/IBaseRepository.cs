namespace NexGoal.AutoService.Repositories
{
    using Fanex.Data.Repository;
    using Models;

    public interface IBaseRepository
    {
        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}