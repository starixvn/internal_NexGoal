namespace NexGoal.BackgroundJob.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criterias.Period;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class PeriodService : IPeriodService
    {
        private readonly IBaseService baseService;

        public PeriodService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> CreatePeriodAsync()
        {
            var criteria = new CreatePeriodCriteria();

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> CreateEmployeeSettingsAsync()
        {
            IEnumerable<int> periodIds = await GetPeriodStartedAsync();
            var criteria = new CreateEmployeeSettingsCriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> GeneratePAAsync()
        {
            IEnumerable<int> periodIds = await GetPeriodStartedAsync();
            var criteria = new GeneratePACriteria
            {
                PeriodIds = string.Join(',', periodIds)
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<IEnumerable<int>> GetPeriodStartedAsync()
        {
            var criteria = new GetPeriodStartedCriteria();

            return await baseService.GetDataAsync<int>(criteria);
        }
    }
}