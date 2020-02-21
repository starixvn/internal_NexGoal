namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedback;

    public interface IPRFeedbackService
    {
        Task<CommandResult> CreatePRFeedback(AddPRFeedbackDto prFeedback);

        Task<CommandResult> EditPRFeedback(EditPRFeedbackDto prFeedback);

        Task<PRFeedbackModel> GetPRFeedback(GetPRFeedbackParam param);
    }
}