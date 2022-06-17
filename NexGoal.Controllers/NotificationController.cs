namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Notification;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        public NotificationController(
            ISessionService sessionService,
            INotificationService notificationService) : base(sessionService)
        {
            this.notificationService = notificationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetNotification(int pageIndex, int pageSize)
        {
            Notification result = await notificationService.GetNotificationAsync(sessionService.EmployeeId, pageIndex, pageSize);

            return Json(result);
        }

        public async Task<JsonResult> ReadNotification(int notificationId)
        {
            CommandResult result = await notificationService.ReadNotificationAsync(notificationId, sessionService.EmployeeId);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> MarkAsRead()
        {
            CommandResult result = await notificationService.MarkAsReadAsync(sessionService.EmployeeId);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetSetting()
        {
            IEnumerable<NotificationSetting> result = await notificationService.GetSettingAsync(sessionService.EmployeeId);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditSetting(IEnumerable<NotificationSetting> settings)
        {
            CommandResult result = await notificationService.EditSettingAsync(sessionService.EmployeeId, settings);

            return Json(result);
        }
    }
}