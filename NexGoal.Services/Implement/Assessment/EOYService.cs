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

    public class EOYService : BaseService, IEOYService
    {
        private readonly IManageNominationRepository manageNominationRepository;

        public EOYService()
            : this(null)
        {
        }

        public EOYService(IManageNominationRepository manageNominationRepository)
        {
            this.manageNominationRepository = manageNominationRepository ?? new ManageNominationRepository();
        }

        public IEnumerable<EOQYModel> GetEOY(GetEOYCriteria criteria)
        {
            IEnumerable<EOQYModel> list = new List<EOQYModel>();

            if (criteria != null && criteria.IsValid())
            {
                list = this.GetData<EOQYModel>(criteria);
                list = list.OrderByDescending(i => i.Year);
            }

            return list;
        }

        public EOQYModel GetEOYById(GetEOYByIdCriteria criteria)
        {
            var result = new EOQYModel();

            if (criteria != null && criteria.IsValid())
            {
                result = this.GetDataEntity<EOQYModel>(criteria);
            }

            return result;
        }

        public IEnumerable<NominatorSettingModel> GetEOYNominatorSettingById(GetEOYNominatorSettingByIdCriteria criteria)
        {
            IEnumerable<NominatorSettingModel> nominators = new List<NominatorSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominators = manageNominationRepository.GetNominatorSettingById(criteria);
            }

            return nominators;
        }
    }
}