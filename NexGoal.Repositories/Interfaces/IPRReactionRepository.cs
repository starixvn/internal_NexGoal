namespace NexGoal.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PRReaction;

    public interface IPRReactionRepository
    {
        Task<PRReactionModel> GetPRReactionByPRId(ICriteria criteria);
    }
}