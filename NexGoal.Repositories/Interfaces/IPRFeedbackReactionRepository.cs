namespace NexGoal.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PRFeedbackReaction;

    public interface IPRFeedbackReactionRepository
    {
        Task<PRFeedbackReactionModel> GetPRFeedbackReactionByFeedbackId(ICriteria criteria);
    }
}