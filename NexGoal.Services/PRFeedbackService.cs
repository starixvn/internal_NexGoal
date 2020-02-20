namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PRFeedback;
    using NexGoal.Criteria.PRReaction;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRReaction;
    using NexGoal.Repositories;

    public class PRFeedbackService : IPRFeedbackService
    {
        private readonly IBaseService baseService;
        private readonly IPRFeedbackRepository prFeedbackRepository;

        public PRFeedbackService(IBaseService baseService, IPRFeedbackRepository prFeedbackRepository)
        {
            this.baseService = baseService;
            this.prFeedbackRepository = prFeedbackRepository;
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

        public async Task<IEnumerable<PRFeedbackModel>> GetPRFeedback(GetPRFeedbackParam param)
        {
            var criteria = new GetPRFeedbackByPRIdCriteria
            {
                PerformanceRecordId = param.PerformanceRecordId,
                ActorID = param.ActorId
            };

            var feedbacks = await prFeedbackRepository.GetPRFeedbacks(criteria);

            return feedbacks;
        }
    }
}