namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.PMSetting;

    public interface IPMSettingService
    {
        Task<IEnumerable<MemberModel>> GetSettingAsync(string pmIds);

        Task<CommandResult> AddMembers(int projectManagerId, string memberIds);
    }
}