namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models;

    public class PAService : IPAService
    {
        private readonly IBaseService baseService;

        public PAService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult EditWhenEvaluationFinished()
        {
            var criteria = new EditWhenEvaluationFinishedCriteria();

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditStatus()
        {
            var criteria = new EditStatusCriteria();

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}