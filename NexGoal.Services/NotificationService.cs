namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Notification;
    using NexGoal.Models.Notification;

    public class NotificationService : INotificationService
    {
        private readonly IBaseService baseService;

        public NotificationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Notification> GetNotificationAsync(int employeeId, int pageIndex, int pageSize)
        {
            var criteria = new GetNotificationByEmployeeCriteria
            {
                EmployeeId = employeeId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var notifications = await baseService.GetDataAsync<NotificationEntity>(criteria).ConfigureAwait(false);

            var notificationsModel = new Notification
            {
                Notifications = notifications,
                UnreadNoti = criteria.UnReadNoti,
                Total = criteria.TotalItems
            };
            return notificationsModel;
        }

        public async Task<IEnumerable<NotificationSetting>> GetSettingAsync(int employeeId)
        {
            var criteria = new GetNotificationSettingCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<NotificationSetting>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSettingAsync(int employeeId, IEnumerable<NotificationSetting> settings)
        {
            var criteria = new UpdateNotificationSettingCriteria
            {
                EmployeeId = employeeId,
                Value = settings.ToXML().ToString()
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> ReadNotificationAsync(int notificationId, int employeeId)
        {
            var criteria = new UpdateNotificationStatusCriteria
            {
                NotificationId = notificationId,
                EmployeeId = employeeId,
                StatusId = (int)NotificationStatus.Read
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> MarkAsReadAsync(int employeeId)
        {
            var criteria = new MarkAsReadNotificationCriteria
            {
                EmployeeId = employeeId,
                StatusId = (int)NotificationStatus.Read
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}