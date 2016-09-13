namespace NexGoal.Site.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class ExportNominationController : Controller
    {
        private readonly INominationService nominationService;
        private readonly ISessionService sessionService;

        public ExportNominationController()
            : this(null, null)
        {
        }

        public ExportNominationController(INominationService nominationService, ISessionService sessionService)
        {
            this.nominationService = nominationService ?? new NominationService();
            this.sessionService = sessionService ?? new SessionService();
        }

        public ActionResult ExportEOQ(GetNominationByNominatorIdCriteria criteria)
        {
            string fileName = criteria.PeriodName + ".xls";

            criteria.NominatorId = sessionService.EmployeeId;
            IEnumerable<NominationViewModel> nominations = nominationService.GetNominationByNominatorId(criteria);

            MemoryStream output = nominationService.ExportEOQ(nominations, criteria.PeriodName);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}