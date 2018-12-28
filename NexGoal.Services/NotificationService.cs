namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Notification;
    using NexGoal.Models.Notification;
    using NexGoal.Services.Interfaces;

    public class NotificationService : INotificationService
    {
        private readonly IBaseService baseService;

        public NotificationService(IBaseService baseService)
        {
            this.baseService = baseService;
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
    }
}