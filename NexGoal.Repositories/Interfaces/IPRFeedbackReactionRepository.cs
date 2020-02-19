namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PRFeedbackReaction;

    public interface IPRFeedbackReactionRepository
    {
        Task<IEnumerable<PRFeedbackReactionModel>> GetPRFeedbackReactionByFeedbackId(ICriteria criteria);
    }
}