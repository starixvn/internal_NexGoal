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

        public DisciplineController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
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

            return Ok(result)
        }
    }
}