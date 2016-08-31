namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Model;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class PAFocusController : BaseController
    {
        private readonly IPAFocusService focusService;

        public PAFocusController()
            : this(null)
        {
        }

        public PAFocusController(IPAFocusService menuService)
        {
            this.focusService = menuService ?? new PAFocusService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.Period, Policy.Read)]
        public IEnumerable<PAFocusProcessSetting> GetPAProcessSettingByPeriodId(int periodId)
        {
            var processSettings = this.focusService.GetPAFocusProcessSettingByPeriodId(periodId);

            return processSettings;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.Period, Policy.Host)]
        public CommandResult UpdatePAProcessSetting(int periodId, IEnumerable<PAFocusProcessSetting> process)
        {
            var result = this.focusService.UpdateProcessSetting(periodId, process);

            return result;
        }
    }
}