namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;

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
        public async Task<JsonResult> GetNominationPeriodsForNominator()
        {
            IEnumerable<NominationPeriodModel> result = await employeeNominationService.GetNominationPeriodsForForComittee(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetNominationsByPeriodId(string nominationPeriodIds)
        {
            IEnumerable<NominationModel> result = await employeeNominationService.GetNominationsByPeriodId(nominationPeriodIds).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditStatus(int nominationId, int statusId)
        {
            var result = await employeeNominationService.EditStatus(nominationId, statusId, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }
    }
}