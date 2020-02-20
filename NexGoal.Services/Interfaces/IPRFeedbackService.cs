namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRReaction;

    public interface IPRFeedbackService
    {
        Task<CommandResult> CreatePRFeedback(AddPRFeedbackDto prFeedback);

        Task<CommandResult> EditPRFeedback(EditPRFeedbackDto prFeedback);

        Task<IEnumerable<PRFeedbackModel>> GetPRFeedback(GetPRFeedbackParam param);
    }
}