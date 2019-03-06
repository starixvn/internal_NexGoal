namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord.Notification;

    public interface ISkypeNotiService
    {
        Task SendMessageMemberToSupervisor(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action);

        Task SendMessageSupervisorToMember(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            ActionOnPerformanceRecord action);

        Task SendMessageSupervisorToSupervisor(Employee actor,
            IEnumerable<ProcessSettingDto> recievers,
            EmployeeDto employee,
            ActionOnPerformanceRecord action);
    }
}