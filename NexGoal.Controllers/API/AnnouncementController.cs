namespace NexGoal.Controllers.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AnnouncementController : BaseController
    {
        private readonly ICommonService _commonService;

        public AnnouncementController(
            ISessionService sessionService,
            ICommonService commonService) : base(sessionService)
        {
            _commonService = commonService;
        }

        [HttpGet]
        public async Task<JsonResult> GetAnnouncements(int id)
        {
            IEnumerable<string> annoucements = await _commonService.GetAnnouncementAsync(id);

            return Json(annoucements);
        }
    }
}