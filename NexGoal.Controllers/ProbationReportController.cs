namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.ProbationReport, Policy = Policy.Read)]
    public class ProbationReportController : BaseController
    {
        public ProbationReportController(ISessionService sessionService) : base(sessionService)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}