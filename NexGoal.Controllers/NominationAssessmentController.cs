namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.EmployeeNomination, Policy = Policy.Read)]
    public class NominationAssessmentController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeNominationService employeeNominationService;

        public NominationAssessmentController(
            ISessionService sessionService,
            IEmployeeNominationService employeeNominationService)
        {
            this.sessionService = sessionService;
            this.employeeNominationService = employeeNominationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrentNominationPeriodForComittee(bool isQuarter)
        {
            NominationPeriodModel result = await employeeNominationService.GetCurrentNominationPeriodForComittee(sessionService.EmployeeId, isQuarter).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetNominationsByPeriodIds(string nominationPeriodIds, int? statusId)
        {
            IEnumerable<NominationModel> result = await employeeNominationService.GetNominationsByPeriodIdsAsync(nominationPeriodIds, statusId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditStatus(int nominationId, int statusId)
        {
            var result = await employeeNominationService.EditStatusAsync(nominationId, statusId, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }
    }
}