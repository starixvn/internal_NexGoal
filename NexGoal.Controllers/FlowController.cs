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
        public async Task<IActionResult> GetFlowTemplates(int? targetTypeId = null, bool? isActive = null)
        {
            IEnumerable<FlowTemplateModel> flowTemplates = await flowTemplateService.GetAllAsync(targetTypeId, isActive).ConfigureAwait(false);

            return Json(flowTemplates);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Host)]
        public async Task<IActionResult> CreateFlowTemplate(CreateFlowTemplateModel flowTemplate)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await flowTemplateService.CreateAsync(flowTemplate).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}