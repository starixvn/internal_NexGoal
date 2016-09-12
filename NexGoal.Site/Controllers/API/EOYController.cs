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

    public class EOYController : BaseController
    {
        private readonly IEOYService eoyService;

        public EOYController()
            : this(null)
        {
        }

        public EOYController(IEOYService eoyService)
        {
            this.eoyService = eoyService ?? new EOYService();
        }

        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Read)]
        public IEnumerable<EOQYModel> GetEOY(int? year = null, int? statusId = null)
        {
            var criteria = new GetEOYCriteria
            {
                Year = year,
                StatusId = statusId
            };

            return eoyService.GetEOY(criteria);
        }

        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Read)]
        public EOQYModel GetEOYById(int id)
        {
            var criteria = new GetEOYByIdCriteria
            {
                EOQYId = id
            };

            return eoyService.GetEOYById(criteria);
        }

        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Read)]
        public IEnumerable<EOQYNominatorSettingModel> GetEOYNominatorSettingById(
            int eoqyId,
            string query = null,
            string groupIds = null,
            int? roleId = null,
            bool showAllEmployee = false)
        {
            var criteria = new GetEOYNominatorSettingByIdCriteria
            {
                EOQYId = eoqyId,
                Query = query,
                GroupIds = groupIds,
                RoleId = roleId,
                ShowAllEmployee = showAllEmployee
            };

            return eoyService.GetEOYNominatorSettingById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult CreateEOY(CreateEOYCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoyService.CreateEOY(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult UpdateEOY(UpdateEOYCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoyService.UpdateEOY(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult UpdateEOYNominatorSettingById(UpdateEOYNominatorSettingByIdCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoyService.UpdateEOYNominatorSettingById(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey.ManageNomination, Policy.Host)]
        public CommandResult DeleteEOY(int id)
        {
            var criteria = new DeleteEOYCriteria
            {
                EOQYId = id,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = eoyService.DeleteEOY(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public EOQYNominatorSettingModel GetCurrentEOYByNominatorId()
        {
            return eoyService.GetCurrentEOYByNominatorId(SessionService.EmployeeId);
        }

        public int GetUserPermissionOnManageNomination()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.ManageNomination);
        }

        public int GetUserPermissionOnNomination()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.Nomination);
        }
    }
}