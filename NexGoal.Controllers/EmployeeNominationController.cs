namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
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

        public async Task<JsonResult> GetNominationPeriodsForNominator()
        {
            IEnumerable<NominationPeriodModel> result = await employeeNominationService.GetNominationPeriodsForNominator(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CreateEmployeeNominationViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeNominationService.Create(new CreateEmployeeNominationModel
                {
                    NominationPeriodId = model.NominationPeriodId,
                    NomineeId = model.NomineeId,
                    NominatorId = sessionService.EmployeeId,
                    Comment = model.Comment
                }).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSuccessful = false, errors });
        }
    }
}