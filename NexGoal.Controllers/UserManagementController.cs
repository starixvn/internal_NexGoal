namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.UserManagement;
    using NexGoal.Models.UserManagement;

    public class UserManagementController : Controller
    {
        private readonly IBaseService baseService;

        public UserManagementController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser)
        {
            var results = await baseService.GetDataAsync<UserModel>(new GetUsersCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsLocal = isLocal,
                IsDisabledUser = isDisabledUser
            }).ConfigureAwait(false);

            return Ok(results);
        }

        public async Task<IActionResult> GetGroups()
        {
            var results = await baseService.GetDataAsync<UserGroupModel>(new GetGroupsCriteria()).ConfigureAwait(false);

            return Ok(results);
        }
    }
}