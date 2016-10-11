namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;

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

        public IEnumerable<NominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria)
        {
            IEnumerable<NominatorSettingModel> nominators = new List<NominatorSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominators = manageNominationRepository.GetNominatorSettingById(criteria);
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

        public IEnumerable<int> GetYearEOQ()
        {
            var criteria = new GetYearEOQCriteria();

            return this.GetData<int>(criteria);
        }
    }
}