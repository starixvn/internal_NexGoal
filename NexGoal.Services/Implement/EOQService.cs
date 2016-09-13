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

    public class EOQService : BaseService, IEOQService
    {
        private readonly IEOQRepository eoqRepository;

        public EOQService()
            : this(null)
        {
        }

        public EOQService(IEOQRepository eoqRepository)
        {
            this.eoqRepository = eoqRepository ?? new EOQRepository();
        }

        public IEnumerable<EOQYModel> GetEOQ(GetEOQCriteria criteria)
        {
            IEnumerable<EOQYModel> list = new List<EOQYModel>();

            if (criteria != null && criteria.IsValid())
            {
                list = this.GetData<EOQYModel>(criteria);
            }

            return list;
        }

        public EOQYModel GetEOQById(GetEOQByIdCriteria criteria)
        {
            var result = new EOQYModel();

            if (criteria != null && criteria.IsValid())
            {
                result = this.GetDataEntity<EOQYModel>(criteria);
            }

            return result;
        }

        public IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria)
        {
            IEnumerable<EOQYNominatorSettingModel> nominators = new List<EOQYNominatorSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominators = eoqRepository.GetEOQNominatorSettingById(criteria);
            }

            return nominators;
        }

        public CommandResult CreateEOQ(CreateEOQCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEOQ(UpdateEOQCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEOQNominatorSettingById(UpdateEOQNominatorSettingByIdCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteEOQ(DeleteEOQCriteria criteria)
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