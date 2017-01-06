namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class EoyController : BaseController
    {
        private readonly IEOYService eoyService;

        public EoyController()
            : this(null)
        {
        }

        public EoyController(IEOYService eoyService)
        {
            this.eoyService = eoyService ?? new EOYService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public IEnumerable<EOQYModel> GetEoy(int? statusId = null)
        {
            var criteria = new GetEOYCriteria
            {
                StatusId = statusId
            };

            return eoyService.GetEOY(criteria);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public EOQYModel GetEoyById(int id)
        {
            var criteria = new GetEOYByIdCriteria
            {
                EOQYId = id
            };

            return eoyService.GetEOYById(criteria);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public IEnumerable<NominatorSettingModel> GetEoyNominatorSettingById(
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
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult CreateEoy(CreateEOYCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoyService.CreateEOY(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult UpdateEoy(UpdateEOYCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoyService.UpdateEOY(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult UpdateEoyNominatorSettingById(UpdateEOYNominatorSettingByIdCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoyService.UpdateEOYNominatorSettingById(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult DeleteEoy(int id)
        {
            var criteria = new DeleteEOYCriteria
            {
                EOQYId = id,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = eoyService.DeleteEOY(criteria);
            return commandResult;
        }
    }
}