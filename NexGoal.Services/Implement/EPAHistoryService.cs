namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EPAHistory;
    using NexGoal.Model.EPAHistory;
    using NexGoal.Model.Period;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class EPAHistoryService : BaseService, IEPAHistoryService
    {
        private readonly IEPAHistoryRepository _epaHistoryRepository;

        public EPAHistoryService()
            : this(null)
        {
        }

        public EPAHistoryService(IEPAHistoryRepository epaHistoryRepository)
        {
            _epaHistoryRepository = epaHistoryRepository ?? new EPAHistoryRepository();
        }

        public IList<EPAHistoryModel> GetEPAHistory(GetEPAHistoryCriteria criteria)
        {
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _epaHistoryRepository.GetEPAHistory(criteria);

                DecodeFinalScoreAndGrade(epas);
            }

            return epas;
        }

        public IList<EPAHistoryModel> GetMonthlyEPAHistory(GetMonthlyEPAHistoryCriteria criteria)
        {
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _epaHistoryRepository.GetMonthlyEPAHistory(criteria);

                DecodeFinalScoreAndGrade(epas);
            }

            return epas;
        }

        private void DecodeFinalScoreAndGrade(IList<EPAHistoryModel> epas)
        {
            foreach (EPAHistoryModel epa in epas)
            {
                epa.Decode();
            }
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            IList<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<PeriodModel>(criteria).ToList();
            }

            return periods;
        }
    }
}