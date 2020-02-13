namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class PAController : BaseController
    {
        private readonly IPARecordService paRecordService;

        public PAController(
            ISessionService sessionService,
            IPARecordService paRecordService) : base(sessionService)
        {
            this.paRecordService = paRecordService;
        }

        public async Task<ActionResult> GetById(int id)
        {
            Policy permission = await paRecordService.GetPermissionOnPerformanceAppraisalAsync(sessionService.EmployeeId, id);
            if (permission == Policy.None)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }

            PARecordModel paRecord = await paRecordService.GetPARecordByIdAsync(id, sessionService.EmployeeId);
            if (paRecord == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Json(paRecord);
        }
    }
}