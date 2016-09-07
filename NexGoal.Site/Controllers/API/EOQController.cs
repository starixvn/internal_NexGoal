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
        public IEnumerable<EOQYModel> GetEOQ(int? year = null, int? statusId = null)
        {
            var criteria = new GetEOQCriteria
            {
                Year = year,
                StatusId = statusId
            };

            return eoqService.GetEOQ(criteria);
        }

        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Read)]
        public EOQYModel GetEOQById(int id)
        {
            var criteria = new GetEOQByIdCriteria
            {
                EOQYId = id
            };

            return eoqService.GetEOQById(criteria);
        }

        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Read)]
        public IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(
            string query = null,
            string groupIds = null,
            int? processId = null,
            bool showAllEmployee = false)
        {
            var criteria = new GetEOQNominatorSettingByIdCriteria
            {
                Query = query,
                GroupIds = groupIds,
                ProcessId = processId,
                ShowAllEmployee = showAllEmployee
            };

            return eoqService.GetEOQNominatorSettingById(criteria);
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

        [HttpPut]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult UpdateEOQNominatorSettingById(UpdateEOQNominatorSettingByIdCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoqService.UpdateEOQNominatorSettingById(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult DeleteEOQ(int id)
        {
            var criteria = new DeleteEOQCriteria
            {
                EOQYId = id,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = eoqService.DeleteEOQ(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnManageNomination()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.ManageNomination);
        }
    }
}