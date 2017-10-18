namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.TargetTemplate;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Model.TargetTemplate;

    public interface ITargetTemplateService
    {
        TargetTemplateModel GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria);

        IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(GetKPIGroupOfTargetTemplateCriteria criteria);

        IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(GetKPIOfTargetTemplateCriteria criteria, bool onlyKPISelected);

        IEnumerable<RatingType> GetRatingType(GetRatingTypeCriteria criteria);
    }
}