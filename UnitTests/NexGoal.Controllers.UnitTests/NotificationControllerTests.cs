namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Notification;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class NotificationControllerTests
    {
        private ISessionService sessionService;
        private INotificationService notificationService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var controller = GetNotificationController();

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetNotification_WhenCalled_ReturnExpectedNotification()
        {
            // Arrange
            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    EmployeeId = 363,
                    CreatorId = 125,
                    Content = "Quinn has created Performance Record for you",
                    StatusId = NotificationStatus.Unread,
                    CreatedDate = DateTime.Now
                }
            };

            var expectResult = new Notification
            {
                Notifications = notifications,
                UnreadNoti = 10,
                Total = 20
            };
            var controller = GetNotificationController();

            notificationService.GetNotificationAsync(sessionService.EmployeeId, 0, 10).Returns(expectResult);

            // Act
            var actual = await controller.GetNotification(0, 10);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<object>(jsonResult.Value);
            Assert.Equal(expectResult, result);
        }

        [Fact]
        public async Task GetSetting_WhenCalled_ReturnExpectedSettingList()
        {
            // Arrange
            var settings = new List<NotificationSetting>
            {
                new NotificationSetting
                {
                    Type = NotificationType.PerformanceAppraisal,
                    ReceiveEmail = true,
                    ReceiveNotification = false
                }
            };
            var controller = GetNotificationController();
            notificationService.GetSettingAsync(sessionService.EmployeeId).Returns(settings);

            // Act
            var actual = await controller.GetSetting();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<NotificationSetting>>(jsonResult.Value);
            Assert.Equal(settings, result);
        }

        [Fact]
        public async Task EditSettingAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var settings = new List<NotificationSetting>
            {
                new NotificationSetting
                {
                    Type = NotificationType.PerformanceAppraisal,
                    ReceiveEmail = true,
                    ReceiveNotification = false
                }
            };

            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };

            var controller = GetNotificationController();
            notificationService.EditSettingAsync(sessionService.EmployeeId, settings).Returns(expected);

            // Act
            var actual = await controller.EditSetting(settings);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ReadNotification_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };

            var controller = GetNotificationController();
            notificationService.ReadNotificationAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.ReadNotification(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task MarkAsRead_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };

            var controller = GetNotificationController();
            notificationService.MarkAsReadAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.MarkAsRead();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, result);
        }

        #region Helpers

        private NotificationController GetNotificationController()
        {
            sessionService = Helpers.FakeSessionService();
            notificationService = Substitute.For<INotificationService>();
            var controller = new NotificationController(sessionService, notificationService);

            return controller;
        }

        #endregion Helpers
    }
}