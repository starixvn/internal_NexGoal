namespace NexGoal.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PRFeedback;
    using NexGoal.Criteria.PRReaction;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Repositories;

    public class PRFeedbackService : IPRFeedbackService
    {
        private readonly IBaseService baseService;
        private readonly IPRFeedbackRepository prFeedbackRepository;
        private readonly ICommonService commonService;

        public PRFeedbackService(IBaseService baseService, IPRFeedbackRepository prFeedbackRepository, ICommonService commonService)
        {
            this.baseService = baseService;
            this.prFeedbackRepository = prFeedbackRepository;
            this.commonService = commonService;
        }

        public async Task<CommandResult> CreatePRFeedback(AddPRFeedbackDto prFeedback)
        {
            var criteria = new AddPRFeedbackCriteria
            {
                PerformanceRecordId = prFeedback.PerformanceRecordId,
                CreatorId = prFeedback.CreatorId,
                Content = commonService.RemoveInvalidXmlChars(prFeedback.Content)
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<CommandResult> EditPRFeedback(EditPRFeedbackDto prFeedback)
        {
            var criteria = new EditPRFeedbackCriteria
            {
                PRFeedbackId = prFeedback.PRFeedbackId,
                CreatorId = prFeedback.CreatorId,
                Content = commonService.RemoveInvalidXmlChars(prFeedback.Content)
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<PRFeedbackModel> GetPRFeedback(GetPRFeedbackParam param)
        {
            var criteria = new GetPRFeedbackByPRIdCriteria
            {
                PerformanceRecordId = param.PerformanceRecordId,
                ActorID = param.ActorId,
                ActorCode = param.ActorCode
            };

            var feedbacks = await prFeedbackRepository.GetPRFeedback(criteria);

            return feedbacks;
        }
    }
}