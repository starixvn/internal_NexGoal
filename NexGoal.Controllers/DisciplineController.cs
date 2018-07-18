namespace NexGoal.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Discipline;
    using NexGoal.Models.Discipline;

    [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Read)]
    public class DisciplineController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public DisciplineController(IBaseService baseService, ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.DisciplineManagement, Policy.Host));
        }

        [HttpGet]
        public async Task<IActionResult> GetDisciplines(DateTime from, DateTime to, bool? isActive, string groupIds, int? employeeId)
        {
            var criteria = new GetDisciplinesCriteria
            {
                FromDate = from,
                ToDate = to,
                IsActive = isActive,
                GroupIds = groupIds,
                EmployeeId = employeeId
            };

            var result = await baseService.GetDataAsync<DisciplineModel>(criteria).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<IActionResult> EditStatus(int disciplineId, bool isActive)
        {
            var criteria = new UpdateDisciplineStatusCriteria
            {
                DisciplineId = disciplineId,
                IsActive = isActive,
                ModifiedBy = sessionService.EmployeeId
            };

            var result = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return Json(result);
        }
    }
}