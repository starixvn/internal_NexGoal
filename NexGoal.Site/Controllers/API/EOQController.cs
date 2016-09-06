namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class EOQController : BaseController
    {
        private readonly IEOQService eoqService;

        public EOQController()
            : this(null)
        {
        }

        public EOQController(IEOQService eoqService)
        {
            this.eoqService = eoqService ?? new EOQService();
        }

        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Read)]
        public IEnumerable<EOQModel> GetEOQ(int? year = null, int? statusId = null)
        {
            var criteria = new GetEOQCriteria
            {
                Year = year,
                StatusId = statusId
            };

            return eoqService.GetEOQ(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult CreateEOQ(CreateEOQCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoqService.CreateEOQ(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult UpdateEOQ(UpdateEOQCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoqService.UpdateEOQ(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult DeleteEOQ(DeleteEOQCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoqService.DeleteEOQ(criteria);

            return commandResult;
        }
    }
}