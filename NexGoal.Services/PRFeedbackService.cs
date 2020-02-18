namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PRFeedback;
    using NexGoal.Criteria.PRFeedbackReaction;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;

    public class PRFeedbackService : IPRFeedbackService
    {
        private readonly IBaseService baseService;

        public PRFeedbackService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> CreatePRFeedback(AddPRFeedbackDto prFeedback)
        {
            var criteria = new AddPRFeedbackCriteria
            {
                PerformanceRecordId = prFeedback.PerformanceRecordId,
                CreatorId = prFeedback.CreatorId,
                Content = prFeedback.Content
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> EditPRFeedback(EditPRFeedbackDto prFeedback)
        {
            var criteria = new EditPRFeedbackCriteria
            {
                PRFeedbackId = prFeedback.PRFeedbackId,
                CreatorId = prFeedback.CreatorId,
                Content = prFeedback.Content
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
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
    }
}