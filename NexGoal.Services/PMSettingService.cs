namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PMSetting;
    using NexGoal.Models.Common;

    public class PMSettingService : IPMSettingService
    {
        private readonly IBaseService baseService;

        public PMSettingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetSettingAsync(string pmIds)
        {
            return await baseService.GetDataAsync<EmployeeBaseModel>(new GetPMSettingCriteria
            {
                PMIds = pmIds
            }).ConfigureAwait(false);
        }
    }
}