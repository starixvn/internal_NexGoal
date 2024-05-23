namespace NexGoal.Controllers.API
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.SyncData;
    using NexGoal.Models.SyncData;

    [ApiController]
    [AllowAnonymous]
    public class SyncFlowController : ControllerBase
    {
        private readonly IBaseService baseService;

        public SyncFlowController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [Route("api/flow/getflow")]
        public async Task<ActionResult> GetFlow()
        {
            var criteria = new GetFlowToSyncCriteria();
            (var groupFlows, var employeeFlows) = await baseService.FetchMultipleAsync<GroupFlow, EmployeeFlow>(criteria);
            return new JsonResult(new { groupFlows, employeeFlows });
        }
    }
}