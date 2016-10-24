namespace NexGoal.Core.Repositories
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;

    public interface IBaseRepository
    {
        CommandResult Insert(ICriteria criteria);

        CommandResult Update(ICriteria criteria);

        CommandResult Delete(ICriteria criteria);
    }
}