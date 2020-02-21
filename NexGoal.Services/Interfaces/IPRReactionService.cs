namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRReaction;

    public interface IPRReactionService
    {
        Task<CommandResult> CreatePRReaction(AddPRReactionDto prReaction);

        Task<CommandResult> RemovePRReaction(RemovePRReactionDto prReaction);

        Task<PRReactionModel> GetByPRId(GetPRReactionParam param);
    }
}