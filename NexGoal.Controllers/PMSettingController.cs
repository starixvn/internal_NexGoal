namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Web.Core.Filters;

    [NexGoalPermission(FunctionKey = FunctionKey.PMSetting, Policy = Policy.Host)]
    public class PMSettingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}