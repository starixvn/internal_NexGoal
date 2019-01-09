namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PMSetting;

    public interface IPMSettingService
    {
        Task<IEnumerable<MemberModel>> GetSettingAsync(string pmIds, int? employeeId);

        Task<CommandResult> AddMembers(int projectManagerId, string memberIds, int createdBy);

        Task<CommandResult> RemoveMember(int projectManagerId, int memberId, int removedBy);

        Task<IEnumerable<EmployeeBaseModel>> GetProjectManagersAsync();
    }
}