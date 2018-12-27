namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Notification;

    public interface INotificationService
    {
        Task<IEnumerable<NotificationSetting>> GetNotificationSetting(int employeeId);
    }
}