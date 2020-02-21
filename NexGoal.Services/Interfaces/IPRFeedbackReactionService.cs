namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedbackReaction;

    public interface IPRFeedbackReactionService
    {
        Task<PRFeedbackReactionModel> GetByFeedbackId(GetPRFeedbackReactionParam param);

        Task<CommandResult> CreatePRFeedbackReaction(AddPRFeedbackReactionDto prFeedbackReaction);

        Task<CommandResult> RemovePRFeedbackReaction(RemovePRFeedbackReactionDto prFeedbackReaction);
    }
}