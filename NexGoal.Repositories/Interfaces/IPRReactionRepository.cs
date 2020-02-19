namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PRReaction;

    public interface IPRReactionRepository
    {
        Task<IEnumerable<PRReactionModel>> GetPRReactionByPRId(ICriteria criteria);
    }
}