namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Probation;
    using NexGoal.Services;
    using NexGoal.ViewModels.Probation;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProbationController : BaseController
    {
        private readonly IProbationService probationService;
        private readonly ICommonService commonService;

        public ProbationController(ISessionService sessionService, IProbationService probationService, ICommonService commonService)
            : base(sessionService)
        {
            this.probationService = probationService;
            this.commonService = commonService;
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> Evaluate(int id)
        {
            var employee = await commonService.GetEmployeeByIdAsync(id).ConfigureAwait(false);
            return View(employee);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> GetEvaluations()
        {
            var result = await probationService.GetProbationEvaluationsAsync(sessionService.EmployeeId, null);

            return Json(result);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> GetEvaluation(int employeeId)
        {
            var result = await probationService.GetProbationEvaluationsAsync(sessionService.EmployeeId, employeeId);

            if (result.Any())
            {
                return Json(result.FirstOrDefault());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> GetEvaluationDetail(int employeeId)
        {
            var result = await probationService.GetEvaluationDetailAsync(employeeId);

            if (result.Any())
            {
                return Json(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> GetHRM(int employeeId)
        {
            var result = await probationService.GetHRM(employeeId);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> EditEvaluatorEvaluation(int employeeId, string comment, bool isQualifiedByEvaluator, int totalScore, IEnumerable<EditEvaluationViewModel> evaluations)
        {
            var result = await probationService.EditEvaluatorEvaluationAsync(new EditEvaluatorEvaluationModel
            {
                EmployeeId = employeeId,
                IsQualifiedByEvaluator = isQualifiedByEvaluator,
                TotalScore = totalScore,
                ActorId = sessionService.EmployeeId,
                Comment = comment,
                Evaluations = evaluations.Select(x => new EditEvaluationModel
                {
                    EmployeeId = x.EmployeeId,
                    KPIId = x.KPIId,
                    Point = x.Point,
                })
            });

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> EditReviewerEvaluation(int employeeId, string comment, bool isQualified)
        {
            var result = await probationService.EditReviewerEvaluationAsync(new EditReviewerEvaluationModel
            {
                EmployeeId = employeeId,
                IsQualified = isQualified,
                ActorId = sessionService.EmployeeId,
                Comment = comment
            });

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
        public async Task<IActionResult> EditSetting(int employeeId, IEnumerable<EditSettingViewModel> settings)
        {
            var result = await probationService.EditSettingAsync(settings.Select(x => new EditProbationSettingModel
            {
                EmployeeId = x.EmployeeId,
                ProcessId = x.ProcessId,
                EvaluatorId = x.EvaluatorId,
                FromDate = x.FromDate,
                ToDate = x.ToDate
            }), employeeId, sessionService.EmployeeId);

            return Json(result);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.ProbationSetting, Policy = Policy.Host)]
        public IActionResult Setting()
        {
            return View();
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.ProbationSetting, Policy = Policy.Host)]
        public async Task<IActionResult> GetProbationers()
        {
            var result = await probationService.GetProbationersAsync();

            return Json(result);
        }
    }
}