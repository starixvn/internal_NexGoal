namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;

    [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Read)]
    public class DisciplineController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}