namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
    }
}