namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.TargetTemplate;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.KPI;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class TargetTemplateService : BaseService, ITargetTemplateService
    {
        private readonly ITargetTemplateRepository _targetTemplateRepository;

        public TargetTemplateService()
            : this(null)
        {
        }

        public TargetTemplateService(ITargetTemplateRepository targetTemplateRepository)
        {
            _targetTemplateRepository = targetTemplateRepository ?? new TargetTemplateRepository();
        }

        public TargetTemplateModel GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria)
        {
            TargetTemplateModel targetTemplate = new TargetTemplateModel();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplate = GetDataEntity<TargetTemplateModel>(criteria);
            }

            return targetTemplate;
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(GetKPIGroupOfTargetTemplateCriteria criteria)
        {
            IEnumerable<KPIGroupModel> groups = new List<KPIGroupModel>();

            if (criteria != null && criteria.IsValid())
            {
                groups = GetData<KPIGroupModel>(criteria);

                foreach (KPIGroupModel group in groups.Where(x => x.Weight > 0))
                {
                    group.Selected = true;
                }
            }

            return groups;
        }

        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(GetKPIOfTargetTemplateCriteria criteria, bool onlyKPISelected)
        {
            IEnumerable<TargetTemplateKPIGroupModel> kpiGroups = new List<TargetTemplateKPIGroupModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<KPIModel> kpiList = _targetTemplateRepository.GetKPIOfTargetTemplate(criteria);
                kpiGroups = GroupKPI(kpiList);

                if (onlyKPISelected)
                {
                    kpiGroups = GetSelectedKPI(kpiGroups);
                }
            }

            return kpiGroups;
        }

        public IEnumerable<RatingType> GetRatingType(GetRatingTypeCriteria criteria)
        {
            IEnumerable<RatingType> ratingTypes = new List<RatingType>();

            if (criteria != null && criteria.IsValid())
            {
                ratingTypes = GetData<RatingType>(criteria);
            }

            return ratingTypes;
        }

        private static bool CheckGroupSelected(List<KPIModel> listKPI)
        {
            bool allSelected = true;
            listKPI.ForEach(x =>
                {
                    if (x.Weight == null)
                    {
                        allSelected = false;
                    }
                });

            return allSelected;
        }

        private static IEnumerable<KPIModel> CheckKPISelected(List<KPIModel> listKPI)
        {
            listKPI.ForEach(x =>
            {
                if (x.Weight >= 0)
                {
                    x.Selected = true;
                }
            });

            return listKPI;
        }

        private static IEnumerable<TargetTemplateKPIGroupModel> GetSelectedKPI(IEnumerable<TargetTemplateKPIGroupModel> kpiGroups)
        {
            return kpiGroups.Where(x => x.KPIs.Any(p => p.Selected.HasValue && p.Selected.Value));
        }

        private IEnumerable<TargetTemplateKPIGroupModel> GroupKPI(IEnumerable<KPIModel> kpiList)
        {
            return kpiList.GroupBy(x => x.KPIGroups.First().KPIGroupId).Select(x => new TargetTemplateKPIGroupModel
            {
                KPIGroupId = x.Key,
                Name = x.First().KPIGroups.First().Name,
                IsSpreadKPI = x.First().KPIGroups.First().IsSpreadKPI,
                Weight = x.First().KPIGroups.First().Weight,
                Selected = CheckGroupSelected(x.ToList()),
                KPIs = CheckKPISelected(x.ToList())
            });
        }
    }
}