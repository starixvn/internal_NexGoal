namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Services.Interfaces;

    public class EOQService : BaseService, IEOQService
    {
        public IEnumerable<EOQModel> GetEOQ(GetEOQCriteria criteria)
        {
            IEnumerable<EOQModel> list = new List<EOQModel>();

            if (criteria != null && criteria.IsValid())
            {
                list = this.GetData<EOQModel>(criteria);
            }

            return list;
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