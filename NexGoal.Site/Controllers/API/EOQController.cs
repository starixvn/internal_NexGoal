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

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public EOQYNominatorSettingModel GetCurrentEOQByNominatorId()
        {
            return eoqService.GetCurrentEOQByNominatorId(SessionService.EmployeeId);
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public IEnumerable<EOQYNominationViewModel> GetEOQNominationByNominatorId(
            int eoqId,
            int roleId,
            string query = null,
            string groupIds = null,
            int? statusId = null)
        {
            var criteria = new GetEOQNominationByNominatorIdCriteria
            {
                EOQYId = eoqId,
                NominatorId = SessionService.EmployeeId,
                RoleId = roleId,
                Query = query,
                GroupIds = groupIds,
                StatusId = statusId
            };

            IEnumerable<EOQYNominationViewModel> nominations = eoqService.GetEOQNominationByNominatorId(criteria);
            return nominations;
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public EOQYNominationModel GetEOQNominationById(int nominationId)
        {
            EOQYNominationModel nomination = eoqService.GetEOQNominationById(nominationId);
            return nomination;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
        public CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                criteria.StatusId = (int)NominationStatus.Nominated;
            }

            CommandResult commandResult = eoqService.CreateEOQNomination(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
        public CommandResult UpdateEOQNomination(UpdateEOQNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                criteria.StatusId = (int)NominationStatus.Nominated;
            }

            CommandResult commandResult = eoqService.UpdateEOQNomination(criteria);
            return commandResult;
        }

        public CommandResult DeleteEOQNomination(int id)
        {
            var criteria = new DeleteEOQNominationCriteria
            {
                NominationId = id,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = eoqService.DeleteEOQNomination(criteria);
            return commandResult;
        }

        public CommandResult UpdateStatusEOQNomination(UpdateStatusEOQNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = eoqService.UpdateStatusEOQNomination(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public IEnumerable<EOQYQuestionModel> GetEOQQuestion()
        {
            return eoqService.GetEOQQuestion();
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