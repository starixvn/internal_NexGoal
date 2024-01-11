namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    public class PRFeedbackReactionController : BaseController
    {
        private readonly IPRFeedbackReactionService prFeedbackReactionService;

        public PRFeedbackReactionController(ISessionService sessionService
            , IPRFeedbackReactionService prFeedbackReactionService)
            : base(sessionService)
        {
            this.prFeedbackReactionService = prFeedbackReactionService;
        }

        [HttpGet]
        public async Task<ActionResult> GetByFeedbackId(GetPRFeedbackReactionParam param)
        {
            param.ActorId = sessionService.EmployeeId;

            var result = await prFeedbackReactionService.GetByFeedbackId(param);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<JsonResult> Create(AddPRFeedbackReactionDto prFeedbackReaction)
        {
            if (ModelState.IsValid)
            {
                prFeedbackReaction.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackReactionService.CreatePRFeedbackReaction(prFeedbackReaction);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });

            return Json(new { isSucessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<JsonResult> Remove(RemovePRFeedbackReactionDto prFeedbackReaction)
        {
            if (ModelState.IsValid)
            {
                prFeedbackReaction.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackReactionService.RemovePRFeedbackReaction(prFeedbackReaction);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });

            return Json(new { isSucessful = false, errors });
        }
    }
}