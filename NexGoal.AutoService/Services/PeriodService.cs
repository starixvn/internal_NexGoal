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

        public CommandResult InsertEmployeeSettings()
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

        public CommandResult EditStatus()
        {
            var criteria = new EditStatusCriteria();

            return baseService.ExecuteNonQuery(criteria);
        }

        private IEnumerable<int> GetPeriodStarted()
        {
            var criteria = new GetPeriodStartedCriteria();

            return baseService.GetData<int>(criteria);
        }
    }
}