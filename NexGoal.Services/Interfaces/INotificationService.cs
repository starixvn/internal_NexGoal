namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Notification;

    public interface INotificationService
    {
        Task<Notification> GetNotification(int employeeId, int pageIndex, int pageSize);

        Task<IEnumerable<NotificationSetting>> GetNotificationSetting(int employeeId);

        Task<CommandResult> ReadNotification(int notificationId, int employeeId);

        Task<CommandResult> MarkAsReadNotification(int employeeId);

        Task<CommandResult> UpdateNotificationSetting(int employeeId, IEnumerable<NotificationSetting> settings);
    }
}