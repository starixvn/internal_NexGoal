namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    public class PRFeedbackController : BaseController
    {
        private readonly IPRFeedbackService prFeedbackService;

        public PRFeedbackController(ISessionService sessionService
            , IPRFeedbackService prFeedbackService)
            : base(sessionService)
        {
            this.prFeedbackService = prFeedbackService;
        }

        [HttpGet]
        public async Task<ActionResult> GetByPRId(GetPRFeedbackParam param)
        {
            param.ActorId = sessionService.EmployeeId;

            var result = await prFeedbackService.GetPRFeedback(param);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<JsonResult> Create(AddPRFeedbackDto prFeedback)
        {
            if (ModelState.IsValid)
            {
                prFeedback.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackService.CreatePRFeedback(prFeedback);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(EditPRFeedbackDto prFeedback)
        {
            if (ModelState.IsValid)
            {
                prFeedback.CreatorId = sessionService.EmployeeId;

                CommandResult commandResult = await prFeedbackService.EditPRFeedback(prFeedback);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}