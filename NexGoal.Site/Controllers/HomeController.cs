namespace NexGoal.Site.Controllers
{
    using System.Web.Mvc;
    using NexGoal.Model.Authentication;
    using NexGoal.Web;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (SessionManager.CurrentUser != null)
            {
                UserSession user = SessionManager.CurrentUser;
                return View(user);
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}