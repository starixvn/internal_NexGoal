namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
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
        public async Task<IActionResult> Create(CreateEmployeeNominationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await employeeNominationService.Create(new CreateEmployeeNominationModel
                {
                    NominationPeriodId = model.NominationPeriodId,
                    NomineeId = model.NomineeId,
                    NominatorId = sessionService.EmployeeId,
                    Comment = model.Comment
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };

            return Json(new { isSuccessful = false, errors });
        }
    }
}