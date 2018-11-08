namespace NexGoal.AutoService.Services
{
    using System.Collections.Generic;
    using NexGoal.AutoService.Criterias.Period;
    using NexGoal.AutoService.Models;

    public class PeriodService : IPeriodService
    {
        private readonly IBaseService baseService;

        public PeriodService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult CreatePeriod()
        {
            var criteria = new CreatePeriodCriteria();

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult CreateEmployeeSettings()
        {
            IEnumerable<int> periodIds = GetPeriodStarted();
            var criteria = new CreateEmployeeSettingsCriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult GeneratePA()
        {
            IEnumerable<int> periodIds = GetPeriodStarted();
            var criteria = new GeneratePACriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        private IEnumerable<int> GetPeriodStarted()
        {
            var criteria = new GetPeriodStartedCriteria();

            return baseService.GetData<int>(criteria);
        }
    }
}