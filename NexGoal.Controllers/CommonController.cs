namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.UserManagement;
    using NexGoal.Models;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;

    public class CommonController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public CommonController(
            IBaseService baseService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string keyword)
        {
            var results = await baseService.GetDataAsync<UserModel>(new SearchUserCriteria { Keyword = keyword }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var results = await baseService.GetDataAsync<UserGroupModel>(new GetGroupsCriteria()).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetTargetTypes()
        {
            string[] refNames = { ReferenceResource.TargetType };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            IEnumerable<TargetTypeModel> targetTypes = referenceResources.Select(x => new TargetTypeModel { TargetTypeId = x.ResourceId, Name = x.Value });

            return Json(targetTypes);
        }
    }
}