namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PMSetting;

    public interface IPMSettingService
    {
        Task<IEnumerable<MemberModel>> GetSettingAsync(string projectManagerIds, int? employeeId = null);

        Task<CommandResult> AddMemberAsync(int projectManagerId, string memberIds, int actorId);

        Task<CommandResult> RemoveMemberAsync(int projectManagerId, int memberId, int actorId);

        Task<IEnumerable<Employee>> GetProjectManagerAsync();
    }
}