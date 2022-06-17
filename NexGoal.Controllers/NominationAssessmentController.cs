namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.NominationSelection, Policy = Policy.Read)]
    public class NominationAssessmentController : BaseController
    {
        private readonly IEmployeeNominationService employeeNominationService;

        public NominationAssessmentController(
            ISessionService sessionService,
            IEmployeeNominationService employeeNominationService) : base(sessionService)
        {
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
            NominationPeriodModel result = await employeeNominationService.GetCurrentNominationPeriodForComitteeAsync(sessionService.EmployeeId, isQuarter);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetNominationsByPeriodIds(string nominationPeriodIds, int? statusId)
        {
            IEnumerable<NominationModel> result = await employeeNominationService.GetNominationsByPeriodIdsAsync(nominationPeriodIds, statusId);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditStatus(int nominationId, int statusId)
        {
            var result = await employeeNominationService.EditStatusAsync(nominationId, statusId, sessionService.EmployeeId);

            return Json(result);
        }
    }
}