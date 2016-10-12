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