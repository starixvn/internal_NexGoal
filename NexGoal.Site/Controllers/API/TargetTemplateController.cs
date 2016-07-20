namespace NexGoal.Site.Controllers.API
{
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
    using System.Collections.Generic;
    using System.Web.Http;

    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateService _targetTemplateService;
        private readonly IScoreRatingTypeService _scoreRatingTypeService;

        public TargetTemplateController()
            : this(null, null)
        {
        }

        public TargetTemplateController(
            ITargetTemplateService targetTemplateService,
            IScoreRatingTypeService scoreRatingTypeService)
        {
            _targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            _scoreRatingTypeService = scoreRatingTypeService ?? new ScoreRatingTypeService();
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Read)]
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

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Read)]
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

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpPost]
        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.CreateTargetTemplate(criteria);
            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _targetTemplateService.UpdateStatusTargetTemplate(criteria);
            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateTargetTemplateBasicInformation(TargetTemplateModel targetTemplate)
        {
            CommandResult commandResult = new CommandResult();

            if (ModelState.IsValid && targetTemplate != null)
            {
                var updateTargetTemplateCriteria = new UpdateTargetTemplateBasicInformationCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    ModifiedBy = CurrentUser.EmployeeId,
                    Name = targetTemplate.Name,
                    TargetTypeId = targetTemplate.TargetTypeId,
                    ScoreRatingTypeId = targetTemplate.ScoreRatingTypeId,
                    RatingTypeId = targetTemplate.RatingTypeId,
                    IsActive = targetTemplate.IsActive,
                    Description = targetTemplate.Description
                };

                commandResult = _targetTemplateService.UpdateTargetTemplateBasicInformation(updateTargetTemplateCriteria);
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateTargetTemplate(TargetTemplateModel targetTemplate)
        {
            CommandResult commandResult = new CommandResult();

            if (ModelState.IsValid && targetTemplate != null)
            {
                var updateTargetTemplateCriteria = new UpdateTargetTemplateCriteria()
                {
                    TargetTemplateId = targetTemplate.TargetTemplateId,
                    ModifiedBy = CurrentUser.EmployeeId,
                    KPIs = targetTemplate.ConvertKPIItemToXml(targetTemplate.KPIIds)
                };

                commandResult = _targetTemplateService.UpdateTargetTemplate(updateTargetTemplateCriteria);
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteTargetTemplate(int targetTemplateId)
        {
            var criteria = new DeleteTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                ModifiedBy = CurrentUser.EmployeeId
            };

            CommandResult commandResult = _targetTemplateService.DeleteTargetTemplate(criteria);
            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpPost]
        public CommandResult AddKPIGroupWeight(TargetKPIGroupWeightModel model)
        {
            CommandResult commandResult = new CommandResult();
            if (model != null)
            {
                var criteria = new SetKPIGroupOfTargetTemplateCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    KPIGroups = model.ConvertKPIGroupWeightToXml,
                    ModifiedBy = CurrentUser.EmployeeId
                };

                commandResult = _targetTemplateService.AddKPIGroupWeight(criteria);
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<KPIGroupModel> groups = _targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);
            return groups;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Read)]
        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(int targetTemplateId, bool onlySelected = false)
        {
            var criteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            IEnumerable<TargetTemplateKPIGroupModel> kpiList = _targetTemplateService.GetKPIOfTargetTemplate(criteria, onlySelected);
            return kpiList;
        }

        /// [ApiAuthorize(EPAFunctionKey.TargetTemplate, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteKPIGroupOfTargetTemplate(int targetTemplateId, int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId,
                KPIGroupId = kpiGroupId,
                ModifiedBy = CurrentUser.EmployeeId
            };

            CommandResult commandResult = _targetTemplateService.DeleteKPIGroup(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetTemplate()
        {
            var permission = Policy.None;
            var isRead = CurrentUser.HasPermission(EPAFunctionKey.TargetTemplate, Policy.Read);
            var isHost = CurrentUser.HasPermission(EPAFunctionKey.TargetTemplate, Policy.Host);
            if (isRead)
            {
                permission = Policy.Read;
            }
            else if (isHost)
            {
                permission = Policy.Host;
            }

            return (int)permission;
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