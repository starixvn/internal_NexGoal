namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Notification;

    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotification(int employeeId);

        Task<IEnumerable<NotificationSetting>> GetNotificationSetting(int employeeId);

        Task<CommandResult> UpdateNotificationSetting(int employeeId, IEnumerable<NotificationSetting> settings);
    }
}