namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;

    [NexGoalPermission(FunctionKey = FunctionKey.PMSetting, Policy = Policy.Host)]
    public class PMSettingController : Controller
    {
        private readonly IPMSettingService pmSettingService;

        public PMSettingController(IPMSettingService pmSettingService)
        {
            this.pmSettingService = pmSettingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSetting(string pmIds)
        {
            var result = await pmSettingService.GetSettingAsync(pmIds).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembers(int projectManagerId, string memberIds)
        {
            return Json(await pmSettingService.AddMembers(projectManagerId, memberIds).ConfigureAwait(false));
        }
    }
}