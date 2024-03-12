namespace NexGoal.Controllers.API
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.AutoService.Criterias.Redmine;
    using NexGoal.AutoService.Models.Redmine;
    using NexGoal.Core.Services;

    [ApiController]
    [AllowAnonymous]
    public class SyncRedmineDataController : ControllerBase
    {
        private readonly IBaseService baseService;

        public SyncRedmineDataController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [Route("api/redmine/sync")]
        public async Task<ActionResult> SyncRedmineData()
        {
            var criteria = new GetLastUpdatedUserCriteria();
            var lastUpdated = await baseService.GetDataEntityAsync<DateTime>(criteria);

            var criteria2 = new GetUserByLastUpdatedCriteria
            {
                LastUpdated = lastUpdated
            };
            var users = await baseService.GetDataAsync<int>(criteria2);

            return new OkResult();
        }
    }
}