namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProbationController : BaseController
    {
        private readonly IProbationService probationService;

        public ProbationController(ISessionService sessionService, IProbationService probationService) : base(sessionService)
        {
            this.probationService = probationService;
        }

        [HttpGet]
        public ActionResult Index(int? id)
        {
            return View(id ?? sessionService.EmployeeId);
        }

        [HttpGet]
        public async Task<IActionResult> GetProbationers(int? employeeId)
        {
            var result = await probationService.GetProbationersAsync(sessionService.EmployeeId, employeeId);

            if (result.Any())
            {
                return Json(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvaluationDetail(int employeeId)
        {
            var result = await probationService.GetEvaluationDetailAsync(employeeId);

            if (result.Any())
            {
                return Json(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}