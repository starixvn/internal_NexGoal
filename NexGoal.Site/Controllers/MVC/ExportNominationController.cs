namespace NexGoal.Site.Controllers
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class ExportNominationController : Controller
    {
        private const string EOQYNominationTemplatePath = @"~\ExcelTemplates\EOQYNomination.xls";

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
            if (criteria != null)
            {
                criteria.NominatorId = sessionService.EmployeeId;
                IEnumerable<NominationViewModel> nominations = nominationService.GetNominationByNominatorId(criteria);

                string templatePath = Server.MapPath(EOQYNominationTemplatePath);
                nominationService.ExportEOQ(nominations, criteria.PeriodName, templatePath);
            }

            return new EmptyResult();
        }

        public ActionResult ExportEOY(GetNominationByNominatorIdCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.NominatorId = sessionService.EmployeeId;
                IEnumerable<NominationViewModel> nominations = nominationService.GetNominationByNominatorId(criteria);

                string templatePath = Server.MapPath(EOQYNominationTemplatePath);
                nominationService.ExportEOY(nominations, criteria.PeriodName, templatePath);
            }

            return new EmptyResult();
        }
    }
}