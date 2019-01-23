namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    public class FlowController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly ICommonService commonService;

        public FlowController(
            ISessionService sessionService,
            ICommonService commonService)
        {
            this.sessionService = sessionService;
            this.commonService = commonService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public IActionResult Index(int? targetTypeId = null, bool? isActive = true)
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.EvaluationFlow, Policy.Host);
            ViewData["IsHost"] = isHost;

            return View();
        }

        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationFlow, Policy = Policy.Read)]
        public async Task<JsonResult> GetEmployeeById(int employeeId)
        {
            Employee employee = await commonService.GetEmployeeByIdAsync(employeeId).ConfigureAwait(false);

            return Json(employee);
        }
    }
}