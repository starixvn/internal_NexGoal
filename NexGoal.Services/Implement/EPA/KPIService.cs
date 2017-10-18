namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.KPI;
    using NexGoal.Model.KPI;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class KPIService : BaseService, IKPIService
    {
        private readonly IKPIRepository _kpiRepository;

        public KPIService()
            : this(null)
        {
        }

        public KPIService(IKPIRepository kpiRepository)
        {
            _kpiRepository = kpiRepository ?? new KPIRepository();
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            IEnumerable<KPIModel> kpis = new List<KPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = _kpiRepository.GetKPI(criteria);
            }

            return kpis;
        }
    }
}