namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Web.Core.Services;

    public class MenuController(ISessionService sessionService) : BaseController (sessionService)
    {
        public JsonResult GetMenuItems()
        {
            var menu = sessionService.MenuItems;

            return Json(menu);
        }
    }
}