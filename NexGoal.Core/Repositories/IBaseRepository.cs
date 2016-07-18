namespace NexGoal.Core.Repositories
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;

    public interface IBaseRepository
    {
        CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false);
    }
}