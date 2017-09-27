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

    public class NominationController : BaseController
    {
        private readonly INominationService nominationService;

        public NominationController()
            : this(null)
        {
        }

        public NominationController(INominationService nominationService)
        {
            this.nominationService = nominationService ?? new NominationService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public NominatorSettingModel GetLatestEoyByNominatorId()
        {
            return nominationService.GetLatestEOYByNominatorId(SessionService.EmployeeId);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public NominatorSettingModel GetLatestEoqByNominatorId()
        {
            return nominationService.GetLatestEOQByNominatorId(SessionService.EmployeeId);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public IEnumerable<QuestionModel> GetEoqQuestion()
        {
            return nominationService.GetEOQQuestion();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public IEnumerable<QuestionModel> GetEoyQuestion()
        {
            return nominationService.GetEOYQuestion();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public IEnumerable<NominationViewModel> GetNominationByNominatorId(
            int eoqyId,
            string query = null,
            string groupIds = null,
            int? statusId = null)
        {
            var criteria = new GetNominationByNominatorIdCriteria
            {
                EOQYId = eoqyId,
                NominatorId = SessionService.EmployeeId,
                Query = query,
                GroupIds = groupIds,
                StatusId = statusId
            };

            IEnumerable<NominationViewModel> nominations = nominationService.GetNominationByNominatorId(criteria);
            return nominations;
        }

        public IEnumerable<NominationEPAModel> GetNominationByPeriodId(int employeeId, int periodId)
        {
            IEnumerable<NominationEPAModel> nominations = nominationService.GetNominationByPeriodId(employeeId, periodId);

            return nominations;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public NominationModel GetEoqNominationById(int nominationId)
        {
            NominationModel nomination = nominationService.GetEOQNominationById(nominationId);
            return nomination;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Read)]
        public NominationModel GetEoyNominationById(int nominationId)
        {
            NominationModel nomination = nominationService.GetEOYNominationById(nominationId);
            return nomination;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Host)]
        public CommandResult CreateEoqNomination(CreateEOQNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                criteria.NominatorId = SessionService.EmployeeId;
                criteria.StatusId = (int)NominationStatus.Nominated;
            }

            CommandResult commandResult = nominationService.CreateEOQNomination(criteria);
            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Host)]
        public CommandResult CreateEoyNomination(CreateEOYNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                criteria.NominatorId = SessionService.EmployeeId;
                criteria.StatusId = (int)NominationStatus.Nominated;
            }

            CommandResult commandResult = nominationService.CreateEOYNomination(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Host)]
        public CommandResult UpdateNomination(UpdateNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                criteria.StatusId = (int)NominationStatus.Nominated;
            }

            CommandResult commandResult = nominationService.UpdateNomination(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Host)]
        public CommandResult DeleteNomination(int id)
        {
            var criteria = new DeleteNominationCriteria
            {
                NominationId = id,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = nominationService.DeleteNomination(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = FunctionKey.Nomination, Policy = Policy.Host)]
        public CommandResult UpdateStatusNomination(UpdateStatusNominationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = nominationService.UpdateStatusNomination(criteria);
            return commandResult;
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