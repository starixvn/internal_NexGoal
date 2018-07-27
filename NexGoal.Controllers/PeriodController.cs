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
    using NexGoal.Criteria;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
    using NexGoal.Models.Period;
    using NexGoal.ViewModels.Period;

    [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
    public class PeriodController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public PeriodController(IBaseService baseService, ISessionService sessionService)
        {
            this.baseService = baseService;
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
            var periods = await baseService.GetDataAsync<PeriodModel>(new GetPeriodsCriteria { TargetTypeId = targetTypeId, IsActive = isActive }).ConfigureAwait(false);

            return Json(periods);
        }

        [HttpGet]
        public async Task<IActionResult> GetTargetTypes()
        {
            var types = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PeriodTarget, IsActive = true }).ConfigureAwait(false);

            return Json(types.Select(x => new TargetTypeModel { TargetTypeId = x.ResourceId, Name = x.Value }));
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreatePeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new CreatePeriodCriteria
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
                };

                var result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }
    }
}