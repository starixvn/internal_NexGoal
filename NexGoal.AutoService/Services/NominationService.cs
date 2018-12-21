namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Criterias.Nomination;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class NominationService : INominationService
    {
        private readonly IBaseService baseService;

        public NominationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult EditWhenFinished()
        {
            var criteria = new EditWhenFinishedCriteria();

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }
    }
}