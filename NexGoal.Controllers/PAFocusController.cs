namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.PAFocus;
    using NexGoal.Services;

    public class PAFocusController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPAFocusService focusService;

        public PAFocusController(
            ISessionService sessionService,
            IPAFocusService focusService)
        {
            this.sessionService = sessionService;
            this.focusService = focusService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPAFocus(GetPAFocusParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PAFocusModel> paFocuses = await focusService.GetPAFocusAsync(param).ConfigureAwait(false);

            return Json(paFocuses);
        }
    }
}