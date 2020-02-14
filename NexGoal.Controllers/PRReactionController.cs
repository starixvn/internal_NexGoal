namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRReaction;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
    public class PRReactionController : BaseController
    {
        private readonly IPRReactionService prReactionService;

        public PRReactionController(ISessionService secsionService, IPRReactionService prReactionService)
            :base(secsionService)
        {
            this.prReactionService = prReactionService;
        }

        [HttpPost]
        public async Task<JsonResult> Create(AddPRReactionDto prReaction)
        {
            if (ModelState.IsValid)
            {
                prReaction.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prReactionService.CreatePRReaction(prReaction).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSucessful = false, errors });
        }

        [HttpPost]
        public async Task<JsonResult> Remove(RemovePRReactionDto prReaction)
        {
            if (ModelState.IsValid)
            {
                prReaction.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prReactionService.RemovePRReaction(prReaction).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSucessful = false, errors });
        }
    }
}