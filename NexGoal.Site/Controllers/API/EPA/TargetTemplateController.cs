namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.ScoreRatingType;
    using NexGoal.Criteria.TargetTemplate;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Model.ScoreRatingType;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateService _targetTemplateService;
        private readonly IScoreRatingTypeService _scoreRatingTypeService;
        private readonly ICommonService commonService;

        public TargetTemplateController()
            : this(null, null, null)
        {
        }

        public TargetTemplateController(
            ITargetTemplateService targetTemplateService,
            IScoreRatingTypeService scoreRatingTypeService,
            ICommonService commonService)
        {
            _targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            _scoreRatingTypeService = scoreRatingTypeService ?? new ScoreRatingTypeService();
            this.commonService = commonService ?? new CommonService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(int? targetTypeId = null, bool? isActive = null)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = null;
            var getAllTargetTemplateCriteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            targetTemplates = _targetTemplateService.GetAllTargetTemplate(getAllTargetTemplateCriteria);
            return targetTemplates;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Read)]
        public TargetTemplateModel GetTargetTemplateById(int targetTemplateId)
        {
            TargetTemplateModel targetTemplate = null;
            var getTargetTemplateByIdCriteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            targetTemplate = _targetTemplateService.GetTargetTemplateById(getTargetTemplateByIdCriteria);
            return targetTemplate;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPost]
        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult UpdateTargetTemplateBasicInformation(TargetTemplateModel targetTemplate)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid && targetTemplate != null)
            {
                var criteria = new UpdateTargetTemplateBasicInformationCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    ModifiedBy = SessionService.EmployeeId,
                    Name = targetTemplate.Name,
                    TargetTypeId = targetTemplate.TargetTypeId,
                    ScoreRatingTypeId = targetTemplate.ScoreRatingTypeId,
                    RatingTypeId = targetTemplate.RatingTypeId,
                    IsActive = targetTemplate.IsActive,
                    Description = targetTemplate.Description
                };

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult UpdateTargetTemplate(TargetTemplateModel targetTemplate)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid && targetTemplate != null)
            {
                var criteria = new UpdateTargetTemplateCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    ModifiedBy = SessionService.EmployeeId,
                    KPIs = targetTemplate.ConvertKPIItemToXml(targetTemplate.KPIIds)
                };

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteTargetTemplate(int targetTemplateId)
        {
            var criteria = new DeleteTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPost]
        public CommandResult AddKPIGroupWeight(TargetKPIGroupWeightModel model)
        {
            var commandResult = new CommandResult();
            if (model != null)
            {
                var criteria = new SetKPIGroupOfTargetTemplateCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    KPIGroups = model.ConvertKPIGroupWeightToXml,
                    ModifiedBy = SessionService.EmployeeId
                };

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<KPIGroupModel> groups = _targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);
            return groups;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(int targetTemplateId, bool onlySelected = false)
        {
            var criteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<TargetTemplateKPIGroupModel> kpiList = _targetTemplateService.GetKPIOfTargetTemplate(criteria, onlySelected);
            return kpiList;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteKPIGroupOfTargetTemplate(int targetTemplateId, int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                KPIGroupId = kpiGroupId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetTemplate()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.EPASetting);
        }

        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(int? scoreRatingTypeId = null, bool isActive = true)
        {
            var criteria = new GetScoreRatingTypeCriteria
            {
                ScoreRatingTypeId = scoreRatingTypeId,
                IsActive = isActive
            };

            IEnumerable<ScoreRatingTypeModel> scoreRatingTypes = _scoreRatingTypeService.GetScoreRatingType(criteria);
            return scoreRatingTypes;
        }

        public IEnumerable<RatingType> GetRatingType()
        {
            var criteria = new GetRatingTypeCriteria();
            IEnumerable<RatingType> ratingTypes = _targetTemplateService.GetRatingType(criteria);
            return ratingTypes;
        }
    }
}