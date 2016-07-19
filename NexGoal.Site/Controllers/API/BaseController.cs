namespace NexGoal.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using NexGoal.Model.Authentication;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using NexGoal.Criteria.Authentication;

    [DeflateCompressionAttribute]
    [ApiAuthorize]
    public class BaseController : ApiController
    {
        public UserSession CurrentUser
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                var currentUser = SessionManager.CurrentUser;
                if (currentUser == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return currentUser;
            }
        }
    }
}