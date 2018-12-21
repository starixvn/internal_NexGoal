namespace NexGoal.AutoService.Services
{
    using System.Collections.Generic;
    using NexGoal.AutoService.Criterias.PAFocus;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class PAFocusService : IPAFocusService
    {
        private readonly IBaseService baseService;

        public PAFocusService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult Generate()
        {
            IEnumerable<int> periodIds = GetPeriodStarted();
            var criteria = new GenerateCriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        private IEnumerable<int> GetPeriodStarted()
        {
            var criteria = new GetPeriodStartedCriteria();

            return baseService.GetDataAsync<int>(criteria).Result;
        }
    }
}