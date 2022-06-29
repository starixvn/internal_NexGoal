namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
    public class MemberNominationController : BaseController
    {
        private readonly IEmployeeNominationService employeeNominationService;

        public MemberNominationController(
            IEmployeeNominationService employeeNominationService,
            ISessionService sessionService) : base(sessionService)
        {
            this.employeeNominationService = employeeNominationService;
        }

        [HttpGet]
        public async Task<JsonResult> GetNominationPeriodsForNominator()
        {
            IEnumerable<NominationPeriodModel> result = await employeeNominationService.GetNominationPeriodsForNominatorAsync(sessionService.EmployeeId);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CreateEmployeeNominationViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeNominationService.CreateAsync(new CreateEmployeeNominationModel
                {
                    NominationPeriodId = model.NominationPeriodId,
                    NomineeId = model.NomineeId,
                    NominatorId = sessionService.EmployeeId,
                    KpiList = model.KpiList
                });

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(EditEmployeeNominationViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeNominationService.EditAsync(new EditEmployeeNominationModel
                {
                    NominationPeriodId = model.NominationPeriodId,
                    NomineeId = model.NomineeId,
                    NominatorId = sessionService.EmployeeId,
                    Comment = model.Comment,
                    PerformanceRecordId = model.PerformanceRecordId
                });

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int performanceRecordId)
        {
            CommandResult commandResult = await employeeNominationService.DeleteAsync(performanceRecordId, sessionService.EmployeeId);

            return Json(commandResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberNominations()
        {
            IEnumerable<PerformanceRecord> result = await employeeNominationService.GetMemberNominationAsync(sessionService.EmployeeId);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberNominationById(int memberId)
        {
            IEnumerable<PerformanceRecord> performanceRecords = await employeeNominationService.GetMemberNominationByIdAsync(sessionService.EmployeeId, memberId);

            return Json(performanceRecords);
        }

        [HttpGet]
        public async Task<IActionResult> GetByRecordId(int performanceRecordId)
        {
            PerformanceRecord performanceRecord = await employeeNominationService.GetNominationByRecordIdAsync(performanceRecordId);

            return Json(performanceRecord);
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplateKIPNomination()
        {
            IEnumerable<TemplateKPINominationInfoModel> templateKPIs = await employeeNominationService.GetTemplateKIPNominationAsync();

            return Json(templateKPIs);
        }
    }
}