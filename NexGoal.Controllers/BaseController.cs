namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Web.Core.Services;

    [Authorize]
    public class BaseController : Controller
    {
        protected readonly ISessionService sessionService;

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }
    }
}