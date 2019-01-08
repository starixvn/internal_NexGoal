namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Notification;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Notification;
    using NexGoal.Services.Interfaces;

    public class NotificationService : INotificationService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public NotificationService(IBaseService baseService
            , ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        public async Task<Notification> GetNotification(int employeeId, int pageIndex, int pageSize)
        {
            var criteria = new GetNotificationByEmployeeCriteria
            {
                EmployeeId = employeeId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var notifications = await baseService.GetDataAsync<NotificationEntity>(criteria).ConfigureAwait(false);

            if (notifications.Any())
            {
                var employees = await commonService.GetEmployeesAsync().ConfigureAwait(false);
                MappingData(notifications, employees);
            }

            var notificationsModel = new Notification
            {
                Notifications = notifications,
                UnreadNoti = criteria.UnReadNoti,
                Total = criteria.TotalItems
            };
            return notificationsModel;
        }

        private static void MappingData(IEnumerable<NotificationEntity> notifications, IEnumerable<EmployeeBaseModel> employees)
        {
            foreach (var notification in notifications)
            {
                notification.Creator = notification.CreatorId == Constants.NexGoalSystemId ?
                NexGoalSystemInfo.NexGoal : employees.FirstOrDefault(x => x.EmployeeId == notification.CreatorId);
            }
        }

        public async Task<IEnumerable<NotificationSetting>> GetNotificationSetting(int employeeId)
        {
            var criteria = new GetNotificationSettingCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<NotificationSetting>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> UpdateNotificationSetting(int employeeId, IEnumerable<NotificationSetting> settings)
        {
            var criteria = new UpdateNotificationSettingCriteria
            {
                EmployeeId = employeeId,
                Value = settings.ToXML().ToString()
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> ReadNotification(int notificationId, int employeeId)
        {
            var criteria = new UpdateNotificationStatusCriteria
            {
                NotificationId = notificationId,
                EmployeeId = employeeId,
                StatusId = (int)NotificationStatus.Read
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> MarkAsReadNotification(int employeeId)
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