namespace NexGoal.Website.Views.Shared.Components.PARecord
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.PARecord;

    public class PARecordViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PAProcessModel pAProcess)
        {
            return View("PARecord", pAProcess);
        }
    }
}