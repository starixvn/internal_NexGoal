namespace NexGoal.Site.Controllers.API
{
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Services;

    [DeflateCompression]
    public class BaseController : ApiController
    {
        private ISessionService sessionService;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? new SessionService();
        }

        public ISessionService SessionService
        {
            get
            {
                return sessionService;
            }

            set
            {
                sessionService = value;
            }
        }
    }
}