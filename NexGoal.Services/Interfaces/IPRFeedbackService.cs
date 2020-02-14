namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;

    public interface IPRFeedbackService
    {
        Task<CommandResult> CreatePRFeedback(AddPRFeedbackDto prFeedback);

        Task<CommandResult> EditPRFeedback(EditPRFeedbackDto prFeedback);

        Task<CommandResult> CreatePRFeedbackReaction(AddPRFeedbackReactionDto prFeedbackReaction);

        Task<CommandResult> RemovePRFeedbackReaction(RemovePRFeedbackReactionDto prFeedbackReaction);
    }
}