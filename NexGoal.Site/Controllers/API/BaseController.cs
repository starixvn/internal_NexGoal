namespace NexGoal.Site.Controllers.API
{
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Services;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            this.SessionService = sessionService ?? new SessionService();
        }

        public ISessionService SessionService { get; set; }
    }
}