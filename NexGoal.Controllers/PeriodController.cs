namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Period;

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

        public async Task<IActionResult> GetPeriods(int? targetTypeId, bool? isActive)
        {
            return Json(new List<TargetTypeModel>());
        }

        public async Task<IActionResult> GetTargetTypes()
        {
            var types = await baseService.GetDataAsync<TargetTypeModel>(new GetTargetTypesCriteria()).ConfigureAwait(false);

            return Json(types);
        }
    }
}