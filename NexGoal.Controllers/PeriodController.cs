namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NexGoal.ViewModels.Period;

    [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
    public class PeriodController : Controller
    {
        private readonly IPeriodService periodService;
        private readonly ISessionService sessionService;

        public PeriodController(IPeriodService periodService, ISessionService sessionService)
        {
            this.periodService = periodService;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.EvaluationPeriod, Policy.Host));
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriods(int? targetTypeId, bool? isActive)
        {
            var periods = await periodService.GetPeriods(targetTypeId, isActive).ConfigureAwait(false);

            return Json(periods);
        }

        [HttpGet]
        public async Task<IActionResult> GetTargetTypes()
        {
            var types = await periodService.GetTargetTypes().ConfigureAwait(false);

            return Json(types);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreatePeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await periodService.Create(new CreatePeriodModel
                {
                    Name = model.Name,
                    TargetTypeId = model.TargetTypeId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DisciplineStartDate = model.DisciplineStartDate,
                    DisciplineEndDate = model.DisciplineEndDate,
                    FocusStartDate = model.FocusStartDate,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedBy = sessionService.EmployeeId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> EditStatus(int periodId, bool isActive)
        {
            var result = await periodService.EditStatus(periodId, isActive, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }
    }
}