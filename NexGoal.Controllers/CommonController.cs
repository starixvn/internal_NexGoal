namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Models;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;

    public class CommonController : Controller
    {
        private readonly ICommonService commonService;

        public CommonController(ICommonService commonService)
        {
            this.commonService = commonService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(
            string keyword = null,
            int? groupId = null,
            bool? isEnabled = true)
        {
            IEnumerable<UserModel> employees = await commonService.GetEmployeesAsync(keyword, groupId, isEnabled).ConfigureAwait(false);

            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            IEnumerable<UserGroupModel> groups = await commonService.GetGroupsAsync().ConfigureAwait(false);

            return Json(groups);
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