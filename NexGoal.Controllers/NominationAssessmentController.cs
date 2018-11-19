namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Services;
    using NexGoal.ViewModels.EOQY;

    public class NominationAssessmentController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeNominationService employeeNominationService;

        public NominationAssessmentController(ISessionService sessionService, IEmployeeNominationService employeeNominationService)
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
        public async Task<JsonResult> GetNominationsByPeriodId(int nominationPeriodId)
        {
            IEnumerable<NominationModel> result = await employeeNominationService.GetNominationsByPeriodId(nominationPeriodId).ConfigureAwait(false);

            return Json(result);
        }
    }
}