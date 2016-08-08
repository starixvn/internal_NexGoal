namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Services;
    using System.Web.Http;

    [DeflateCompressionAttribute]
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