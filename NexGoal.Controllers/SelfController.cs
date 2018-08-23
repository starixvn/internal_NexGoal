namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class SelfController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}