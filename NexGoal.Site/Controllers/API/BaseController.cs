namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Model;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web.Http;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        private readonly ISessionService sessionService;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? new SessionService();
        }

        public UserSession CurrentUser
        {
            get
            {
                return this.sessionService.CurrentUser;
            }
        }
    }
}