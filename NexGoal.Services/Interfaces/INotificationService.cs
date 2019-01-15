namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Notification;

    public interface INotificationService
    {
        Task<Notification> GetNotificationAsync(int employeeId, int pageIndex, int pageSize);

        Task<IEnumerable<NotificationSetting>> GetSettingAsync(int employeeId);

        Task<CommandResult> ReadNotificationAsync(int notificationId, int employeeId);

        Task<CommandResult> MarkAsReadAsync(int employeeId);

        Task<CommandResult> EditSettingAsync(int employeeId, IEnumerable<NotificationSetting> settings);
    }
}