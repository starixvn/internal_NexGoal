namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories.Implement;
    using Repositories.Interfaces;

    public class EOQService : BaseService, IEOQService
    {
        private readonly IManageNominationRepository manageNominationRepository;

        public EOQService()
            : this(null)
        {
        }

        public EOQService(IManageNominationRepository manageNominationRepository)
        {
            this.manageNominationRepository = manageNominationRepository ?? new ManageNominationRepository();
        }

        public IEnumerable<EOQYModel> GetEOQ(GetEOQCriteria criteria)
        {
            IEnumerable<EOQYModel> list = new List<EOQYModel>();

            if (criteria != null && criteria.IsValid())
            {
                list = GetData<EOQYModel>(criteria);
                list = list.OrderByDescending(i => i.Year).ThenByDescending(i => i.Quarter);
            }

            return list;
        }

        public EOQYModel GetEOQById(GetEOQByIdCriteria criteria)
        {
            var result = new EOQYModel();

            if (criteria != null && criteria.IsValid())
            {
                result = GetDataEntity<EOQYModel>(criteria);
            }

            return result;
        }

        public IEnumerable<NominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria)
        {
            IEnumerable<NominatorSettingModel> nominators = new List<NominatorSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominators = manageNominationRepository.GetNominatorSettingById(criteria);
            }

            return nominators;
        }

        public IEnumerable<int> GetYearEOQ()
        {
            var criteria = new GetYearEOQCriteria();

            return GetData<int>(criteria);
        }
    }
}