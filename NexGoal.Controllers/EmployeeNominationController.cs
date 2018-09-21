namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.EmployeeNomination, Policy = Policy.Host)]
    public class EmployeeNominationController : Controller
    {
        private readonly IEmployeeNominationService employeeNominationService;
        private readonly ISessionService sessionService;

        public EmployeeNominationController(IEmployeeNominationService employeeNominationService, ISessionService sessionService)
        {
            this.employeeNominationService = employeeNominationService;
            this.sessionService = sessionService;
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> GetNominationPeriodsForNominator()
        {
            var result = await employeeNominationService.GetNominationPeriodsForNominator(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public IActionResult Create(int nominationPeriodId, int employeeId)
        {
            return Json(new { isSuccessful = true });
        }
    }
}