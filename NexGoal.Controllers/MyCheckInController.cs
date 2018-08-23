namespace NexGoal.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    public class MyCheckInController : Controller
    {
        [Route("Self/CheckIn/Index")]
        public IActionResult Index(
            DateTime fromDate,
            DateTime toDate,
            int? creatorId = null,
            string projectIds = null)
        {
            return View();
        }

        [Route("Self/CheckIn/Create")]
        public IActionResult Create()
        {
            return View();
        }
    }
}