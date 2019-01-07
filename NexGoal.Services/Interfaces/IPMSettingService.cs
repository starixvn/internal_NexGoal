namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;

    public interface IPMSettingService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetSettingAsync(string pmIds);
    }
}