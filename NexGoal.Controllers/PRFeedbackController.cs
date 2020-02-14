namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
    public class PRFeedbackController : BaseController
    {
        private readonly IPRFeedbackService prFeedbackService;

        public PRFeedbackController(ISessionService sessionService, IPRFeedbackService prFeedbackService) 
            : base(sessionService)
        {
            this.prFeedbackService = prFeedbackService;
        }

        [HttpPost]
        public async Task<JsonResult> Create(AddPRFeedbackDto prFeedback)
        {
            if (ModelState.IsValid)
            {
                prFeedback.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackService.CreatePRFeedback(prFeedback).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(EditPRFeedbackDto prFeedback)
        {
            if(ModelState.IsValid)
            {
                prFeedback.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackService.EditPRFeedback(prFeedback).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    
        [Route("PRFeedbackReaction/Create")]
        [HttpPost]
        public async Task<JsonResult> CreatePRFeedbackReaction(AddPRFeedbackReactionDto prFeedbackReaction)
        {
            if (ModelState.IsValid)
            {
                prFeedbackReaction.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackService.CreatePRFeedbackReaction(prFeedbackReaction).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSucessful = false, errors });
        }

        [Route("PRFeedbackReaction/Remove")]
        [HttpPost]
        public async Task<JsonResult> RemovePRFeedbackReaction(RemovePRFeedbackReactionDto prFeedbackReaction)
        {
            if (ModelState.IsValid)
            {
                prFeedbackReaction.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackService.RemovePRFeedbackReaction(prFeedbackReaction).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSucessful = false, errors });
        }
    }
}