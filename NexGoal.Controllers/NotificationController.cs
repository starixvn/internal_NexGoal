namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Notification;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.Core.Services;

    public class NotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly ISessionService sessionService;

        public NotificationController(ISessionService sessionService,
            INotificationService notificationService)
        {
            this.sessionService = sessionService;
            this.notificationService = notificationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetNotification(int pageIndex, int pageSize)
        {
            var result = await notificationService.GetNotification(sessionService.EmployeeId, pageIndex, pageSize)
                .ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> ReadNotification(int notificationId)
        {
            var result = await notificationService.ReadNotification(notificationId, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> MarkAsReadNotification()
        {
            var result = await notificationService.MarkAsReadNotification(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetNotificationSetting()
        {
            var result = await notificationService.GetNotificationSetting(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateNotiSetting(IEnumerable<NotificationSetting> settings)
        {
            var result = await notificationService.UpdateNotificationSetting(sessionService.EmployeeId, settings).ConfigureAwait(false);

            return Json(result);
        }
    }
}