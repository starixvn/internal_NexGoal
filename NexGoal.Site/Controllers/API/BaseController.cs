namespace NexGoal.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using NexGoal.Model.Authentication;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        protected UserSession CurrentUser
        {
            get
            {
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