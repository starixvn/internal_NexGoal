namespace NexGoal.Controllers.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;

    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AnnouncementController : ControllerBase
    {
        private readonly ICommonService _commonService;

        public AnnouncementController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAnnouncements(int id)
        {
            IEnumerable<string> annoucements = await _commonService.GetAnnouncementAsync(id).ConfigureAwait(false);

            return Ok(annoucements);
        }
    }
}