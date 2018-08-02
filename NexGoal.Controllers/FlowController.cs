namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.FlowTemplate;
    using NexGoal.Services;
    using NexGoal.ViewModels.FlowTemplate;

    public class FlowController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IFlowTemplateService flowTemplateService;

        public FlowController(
            ISessionService sessionService,
            IFlowTemplateService flowTemplateService)
        {
            this.sessionService = sessionService;
            this.flowTemplateService = flowTemplateService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public IActionResult Index(int? targetTypeId = null, bool? isActive = true)
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.EvaluationFlow, Policy.Host);
            var viewModel = new FlowTemplateViewModel
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive,
                IsHost = isHost
            };

            return View(viewModel);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(int? targetTypeId = null, bool? isActive = null)
        {
            IEnumerable<FlowTemplateModel> flowTemplates = await flowTemplateService.GetAllAsync(targetTypeId, isActive).ConfigureAwait(false);

            return Json(flowTemplates);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreateFlowTemplateModel flowTemplate)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await flowTemplateService.CreateAsync(flowTemplate).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            FlowTemplateModel flowTemplate = await flowTemplateService.GetByIdAsync(id).ConfigureAwait(false);

            if (flowTemplate == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.EvaluationFlow, Policy.Host);
            ViewData["IsHost"] = isHost;

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(EditFlowTemplateModel flowTemplate)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await flowTemplateService.EditAsync(flowTemplate).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> EditStatus(int flowTemplateId, bool isActive)
        {
            CommandResult result = await flowTemplateService.EditStatusAsync(flowTemplateId, isActive).ConfigureAwait(false);

            return Json(result);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int flowTemplateId)
        {
            CommandResult commandResult = await flowTemplateService.DeleteAsync(flowTemplateId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> Details(int id)
        {
            FlowTemplateModel flowTemplate = await flowTemplateService.GetByIdAsync(id).ConfigureAwait(false);

            if (flowTemplate == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            FlowTemplateModel flowTemplate = await flowTemplateService.GetByIdAsync(id).ConfigureAwait(false);

            if (flowTemplate == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return Json(flowTemplate);
        }
    }
}