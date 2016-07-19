namespace NexGoal.Site.Controllers
{
    using NexGoal.Model.Authentication;
    using NexGoal.Web;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            UserSession user = SessionManager.CurrentUser;
            return View(user);
        }
    }
}