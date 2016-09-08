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

        public EOQYNominatorSettingModel GetCurrentEOQByNominatorId(int nominatorId)
        {
            var criteria = new GetCurrentEOQByNominatorIdCriteria
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

        public IEnumerable<EOQYNominationModel> GetEOQNominationByNominatorId(GetEOQNominationByNominatorIdCriteria criteria)
        {
            IEnumerable<EOQYNominationModel> nominations = new List<EOQYNominationModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominations = eoqRepository.GetNominationByNominatorId(criteria);
            }

            return nominations;
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

        public IEnumerable<EOQYQuestionModel> GetEOQQuestion()
        {
            var criteria = new GetEOQQuestionCriteria();
            IEnumerable<EOQYQuestionModel> questions = this.GetData<EOQYQuestionModel>(criteria);

            return questions;
        }

        public CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public EOQYNominationModel GetEOQNominationById(int nominationId)
        {
            var nomination = new EOQYNominationModel();
            var criteria = new GetEOQNominationByIdCriteria
            {
                NominationId = nominationId
            };

            if (criteria.IsValid())
            {
                nomination = eoqRepository.GetEOQNominationById(criteria);
            }

            return nomination;
        }

        public CommandResult UpdateEOQNomination(UpdateEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }
    }
}