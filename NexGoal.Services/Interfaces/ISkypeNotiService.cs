namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord.Notification;

    public interface ISkypeNotiService
    {
        Task SendMessageMemberToSupervisorAsync(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action);

        Task SendMessageSupervisorToMemberAsync(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action);

        Task SendMessageSupervisorToSupervisorAsync(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            EmployeeDto employee,
            ActionOnPerformanceRecord action);
    }
}