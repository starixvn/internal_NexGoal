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

    public class EoqController : BaseController
    {
        private readonly IEOQService eoqService;
        private readonly ICommonService commonService;

        public EoqController()
            : this(null, null)
        {
        }

        public EoqController(IEOQService eoqService, ICommonService commonService)
        {
            this.eoqService = eoqService ?? new EOQService();
            this.commonService = commonService ?? new CommonService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public IEnumerable<EOQYModel> GetEoq(int? year = null, int? statusId = null)
        {
            var criteria = new GetEOQCriteria
            {
                Year = year,
                StatusId = statusId
            };

            return eoqService.GetEOQ(criteria);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public EOQYModel GetEoqById(int id)
        {
            var criteria = new GetEOQByIdCriteria
            {
                EOQYId = id
            };

            return eoqService.GetEOQById(criteria);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public IEnumerable<int> GetYearEoq()
        {
            return eoqService.GetYearEOQ();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Read)]
        public IEnumerable<NominatorSettingModel> GetEoqNominatorSettingById(
            int eoqyId,
            string query = null,
            string groupIds = null,
            int? roleId = null,
            bool showAllEmployee = false)
        {
            var criteria = new GetEOQNominatorSettingByIdCriteria
            {
                EOQYId = eoqyId,
                Query = query,
                GroupIds = groupIds,
                RoleId = roleId,
                ShowAllEmployee = showAllEmployee
            };

            return eoqService.GetEOQNominatorSettingById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult CreateEoq(CreateEOQCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult UpdateEoq(UpdateEOQCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult UpdateEoqNominatorSettingById(UpdateEOQNominatorSettingByIdCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = FunctionKey.ManageNomination, Policy = Policy.Host)]
        public CommandResult DeleteEoq(int id)
        {
            var criteria = new DeleteEOQCriteria
            {
                EOQYId = id,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }
    }
}