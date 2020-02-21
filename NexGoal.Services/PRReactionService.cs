namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PRReaction;
    using NexGoal.Models.PRReaction;
    using NexGoal.Repositories;

    public class PRReactionService : IPRReactionService
    {
        private readonly IBaseService baseService;
        private readonly IPRReactionRepository prReactionRepository;

        public PRReactionService(IBaseService baseService, IPRReactionRepository prReactionRepository)
        {
            this.baseService = baseService;
            this.prReactionRepository = prReactionRepository;
        }

        public async Task<CommandResult> CreatePRReaction(AddPRReactionDto prReaction)
        {
            var criteria = new AddPRReactionCriteria
            {
                PerformanceRecordId = prReaction.PerformanceRecordId,
                CreatorId = prReaction.CreatorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<CommandResult> RemovePRReaction(RemovePRReactionDto prReaction)
        {
            var criteria = new RemovePRReactionCriteria
            {
                PerformanceRecordId = prReaction.PerformanceRecordId,
                CreatorId = prReaction.CreatorId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<PRReactionModel> GetByPRId(GetPRReactionParam param)
        {
            var criteria = new GetPRReactionByPRIdCriteria
            {
                PerformanceRecordId = param.PerformanceRecordId,
                ActorID = param.ActorId,
                ActorCode = param.ActorCode
            };

            var reactions = await prReactionRepository.GetPRReactionByPRId(criteria);

            return reactions;
        }
    }
}