namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PRFeedbackReaction;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Repositories;

    public class PRFeedbackReactionService : IPRFeedbackReactionService
    {
        private readonly IBaseService baseService;
        private readonly IPRFeedbackReactionRepository prFeedbackReactionRepository;

        public PRFeedbackReactionService(IBaseService baseService,
            IPRFeedbackReactionRepository prFeedbackReactionRepository)
        {
            this.baseService = baseService;
            this.prFeedbackReactionRepository = prFeedbackReactionRepository;
        }

        public async Task<CommandResult> CreatePRFeedbackReaction(AddPRFeedbackReactionDto prFeedbackReaction)
        {
            var criteria = new AddPRFeedbackReactionCriteria
            {
                PRFeedbackId = prFeedbackReaction.PRFeedbackId,
                CreatorId = prFeedbackReaction.CreatorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> RemovePRFeedbackReaction(RemovePRFeedbackReactionDto prFeedbackReaction)
        {
            var criteria = new RemovePRFeedbackReactionCriteria
            {
                PRFeedbackId = prFeedbackReaction.PRFeedbackId,
                CreatorId = prFeedbackReaction.CreatorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<IEnumerable<PRFeedbackReactionModel>> GetPRFeedbackReaction(GetPRFeedbackReactionParam param)
        {
            var criteria = new GetPRFeedbackReactionCriteria
            {
                PRFeedbackId = param.PRFeedbackId,
                ActorId = param.ActorId
            };

            var reactions = await prFeedbackReactionRepository.GetPRFeedbackReactionByFeedbackId(criteria);

            return reactions;
        }
    }
}