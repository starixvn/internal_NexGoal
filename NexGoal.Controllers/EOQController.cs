namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Models.EOQY;
    using NexGoal.Services;
    using NexGoal.ViewModels.EOQY;

    public class EOQController : Controller
    {
        private readonly IEOQService eoqService;

        public EOQController(IEOQService eoqService)
        {
            this.eoqService = eoqService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllEOQ(int? year = null, int? statusId = null)
        {
            IEnumerable<EOQYModel> eoqs = await eoqService.GetAllEOQAsync(year, statusId).ConfigureAwait(false);
            IEnumerable<EOQYViewModel> viewModels = eoqs != null ? eoqs.Select(x => new EOQYViewModel
            {
                EOQYId = x.EOQYId,
                Year = x.Year,
                Quarter = x.Quarter,
                Description = x.Description,
                StartDateForNominator = x.StartDateForNominator,
                EndDateForNominator = x.EndDateForNominator,
                RemindDateForNominator = x.RemindDateForNominator,
                EndDateForCommittee = x.EndDateForCommittee
            }) : new List<EOQYViewModel>();

            return Json(viewModels);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetYears()
        {
            IEnumerable<int> years = await eoqService.GetYearsAsync().ConfigureAwait(false);

            return Json(years);
        }
    }
}