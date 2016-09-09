namespace NexGoal.Site.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class ExportEOQYController : Controller
    {
        private const string EOQTemplatePath = @"~\SourceApp\Components\Assessment\Nomination\ExcelTemplates\EOQ.xls";

        private readonly IEOQService eoqService;
        private readonly ISessionService sessionService;

        public ExportEOQYController()
            : this(null, null)
        {
        }

        public ExportEOQYController(IEOQService eoqService, ISessionService sessionService)
        {
            this.eoqService = eoqService ?? new EOQService();
            this.sessionService = sessionService ?? new SessionService();
        }

        public ActionResult ExportEOQ(GetEOQNominationByNominatorIdCriteria criteria)
        {
            string fileName = "EOQ_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            criteria.NominatorId = sessionService.EmployeeId;
            IEnumerable<EOQYNominationViewModel> nominations = eoqService.GetEOQNominationByNominatorId(criteria);

            MemoryStream output = eoqService.ExportEOQ(nominations, criteria.PeriodName);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}