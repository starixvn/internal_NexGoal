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
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public IActionResult Edit(int id)
        {
            return View(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriods(int? targetTypeId, bool? isActive)
        {
            var periods = await periodService.GetPeriods(targetTypeId, isActive).ConfigureAwait(false);

            return Json(periods);
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriodProcessSetting(int id)
        {
            var processSettings = await periodService.GetPeriodProcessSetting(id).ConfigureAwait(false);

            return Json(processSettings);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var period = await periodService.GetPeriod(id).ConfigureAwait(false);

            return Json(period);
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
                    FocusEndDate = model.FocusEndDate,
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

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int periodId)
        {
            var result = await periodService.Delete(periodId, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(EditPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await periodService.Edit(new EditPeriodModel
                {
                    PeriodId = model.PeriodId,
                    Name = model.Name,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DisciplineStartDate = model.DisciplineStartDate,
                    DisciplineEndDate = model.DisciplineEndDate,
                    FocusStartDate = model.FocusStartDate,
                    FocusEndDate = model.FocusEndDate,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    ModifiedBy = sessionService.EmployeeId
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
        public async Task<IActionResult> EditProcessSetting(int periodId, IEnumerable<PeriodProcessSettingModel> settings)
        {
            var result = await periodService.EditProcessSetting(periodId,
            settings.Select(x => new EditProcessSettingModel
            {
                PeriodId = x.PeriodId,
                ProcessId = x.ProcessId,
                Duration = x.Duration,
                FocusDay = x.FocusDay,
                NoticeAtBegin = x.NoticeAtBegin,
                RemindDay = x.RemindDay
            }), sessionService.EmployeeId)
            .ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeSettings(int periodId, int? groupId, bool? isLocal, bool? isEvaluated, int? statusId)
        {
            var result = await periodService.GetEmployeeSettings(periodId, groupId, isLocal, isEvaluated, statusId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId)
        {
            var result = await periodService.CreateEmployeeSettings(periodId, groupIds, employeeId, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> EditEmployeeSetting(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingViewModel> processes)
        {
            var serviceModels = processes.Select(x => new EditEmployeeSettingModel
            {
                ProcessId = x.ProcessId,
                EvaluatorId = x.EvaluatorId,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });

            var result = await periodService.EditEmployeeSettings(periodId, paRecordIds, serviceModels, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }
    }
}