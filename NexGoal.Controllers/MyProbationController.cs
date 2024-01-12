namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Probation;
    using NexGoal.Services;
    using NexGoal.ViewModels.Probation;
    using NexGoal.Web.Core.Services;

    public class MyProbationController : BaseController
    {
        private readonly IProbationService probationService;

        public MyProbationController(
            ISessionService sessionService,
            IProbationService probationService) : base(sessionService)
        {
            this.probationService = probationService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvaluation()
        {
            var result = await probationService.GetProbationEvaluationsAsync(sessionService.EmployeeId, sessionService.EmployeeId);
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
        public async Task<IActionResult> GetEvaluationDetail()
        {
            var result = await probationService.GetEvaluationDetailAsync(sessionService.EmployeeId);
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
        public async Task<IActionResult> GetHRM()
        {
            var result = await probationService.GetHRM(sessionService.EmployeeId);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditSelfEvaluation(string comment, int totalScore, IEnumerable<EditEvaluationViewModel> evaluations)
        {
            var result = await probationService.EditSelfEvaluationAsync(new EditSelfEvaluationModel
            {
                EmployeeId = sessionService.EmployeeId,
                Comment = comment,
                TotalScore = totalScore,
                Evaluations = evaluations.Select(x => new EditEvaluationModel
                {
                    EmployeeId = x.EmployeeId,
                    KPIId = x.KPIId,
                    Point = x.Point
                })
            });

            return Json(result);
        }
    }
}