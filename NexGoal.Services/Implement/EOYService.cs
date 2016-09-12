namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class EOYService : BaseService, IEOYService
    {
        private readonly IEOQRepository eoqRepository;

        public EOYService()
            : this(null)
        {
        }

        public EOYService(IEOQRepository eoqRepository)
        {
            this.eoqRepository = eoqRepository ?? new EOQRepository();
        }

        public IEnumerable<EOQYModel> GetEOY(GetEOYCriteria criteria)
        {
            IEnumerable<EOQYModel> list = new List<EOQYModel>();

            if (criteria != null && criteria.IsValid())
            {
                list = this.GetData<EOQYModel>(criteria);
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

        public IEnumerable<EOQYNominatorSettingModel> GetEOYNominatorSettingById(GetEOYNominatorSettingByIdCriteria criteria)
        {
            IEnumerable<EOQYNominatorSettingModel> nominators = new List<EOQYNominatorSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominators = eoqRepository.GetEOQNominatorSettingById(criteria);
            }

            return nominators;
        }

        public EOQYNominatorSettingModel GetCurrentEOYByNominatorId(int nominatorId)
        {
            var criteria = new GetCurrentEOYByNominatorIdCriteria
            {
                NominatorId = nominatorId
            };

            var result = new EOQYNominatorSettingModel();

            if (criteria.IsValid())
            {
                result = eoqRepository.GetCurrentEOQByNominatorId(criteria);
            }

            return result;
        }

        public CommandResult CreateEOY(CreateEOYCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEOY(UpdateEOYCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEOYNominatorSettingById(UpdateEOYNominatorSettingByIdCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteEOY(DeleteEOYCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}