namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using System.Threading.Tasks;

    public class ProbationController : BaseController
    {
        private readonly IProbationService probationService;

        public ProbationController(ISessionService sessionService, IProbationService probationService) : base(sessionService)
        {
            this.probationService = probationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProbationersAsync()
        {
            var result = await probationService.GetProbationersAsync(sessionService.EmployeeId);
            return Json(result);
        }
    }
}