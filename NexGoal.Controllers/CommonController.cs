namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Common;
    using NexGoal.Models.UserManagement;

    public class CommonController : Controller
    {
        private readonly IBaseService baseService;

        public CommonController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string keyword)
        {
            var results = await baseService.GetDataAsync<UserModel>(new SearchUserCriteria { Keyword = keyword }).ConfigureAwait(false);

            return Ok(results);
        }
    }
}