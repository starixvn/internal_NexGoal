using System;

namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PRReaction;
    using NexGoal.Models.PRReaction;

    public class PRReactionService : IPRReactionService
    {
        private readonly IBaseService baseService;

        public PRReactionService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> CreatePRReaction(AddPRReactionDto prReaction)
        {
            var criteria = new AddPRReactionCriteria
            {
                PerformanceRecordId = prReaction.PerformanceRecordId,
                CreatorId = prReaction.CreatorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> RemovePRReaction(RemovePRReactionDto prReaction)
        {
            var criteria = new RemovePRReactionCriteria
            {
                PRReactionId = prReaction.PRReactionId,
                CreatorId = prReaction.CreatorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }
    }
}