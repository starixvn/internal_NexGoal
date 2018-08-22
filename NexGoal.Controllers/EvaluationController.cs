namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models;
    using NexGoal.Models.Function;
    using NexGoal.Services;
    using NexGoal.ViewModels.Function;

    public class EvaluationController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        
    }
}