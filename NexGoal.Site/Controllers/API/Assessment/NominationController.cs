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

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public NominatorSettingModel GetLatestEOYByNominatorId()
        {
            return nominationService.GetLatestEOYByNominatorId(SessionService.EmployeeId);
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public NominatorSettingModel GetLatestEOQByNominatorId()
        {
            return nominationService.GetLatestEOQByNominatorId(SessionService.EmployeeId);
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public IEnumerable<QuestionModel> GetEOQQuestion()
        {
            return nominationService.GetEOQQuestion();
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public IEnumerable<QuestionModel> GetEOYQuestion()
        {
            return nominationService.GetEOYQuestion();
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
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
            var criteria = new GetNominationByPeriodIdCriteria
            {
                PeriodId = periodId,
                NomineeId = employeeId
            };

            IEnumerable<NominationEPAModel> nominations = nominationService.GetNominationByPeriodId(criteria);

            return nominations;
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public NominationModel GetEOQNominationById(int nominationId)
        {
            NominationModel nomination = nominationService.GetEOQNominationById(nominationId);
            return nomination;
        }

        [ApiAuthorize(FunctionKey.Nomination, Policy.Read)]
        public NominationModel GetEOYNominationById(int nominationId)
        {
            NominationModel nomination = nominationService.GetEOYNominationById(nominationId);
            return nomination;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
        public CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria)
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
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
        public CommandResult CreateEOYNomination(CreateEOQNominationCriteria criteria)
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

        [HttpPut]
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
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
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
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
        [ApiAuthorize(FunctionKey.Nomination, Policy.Host)]
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