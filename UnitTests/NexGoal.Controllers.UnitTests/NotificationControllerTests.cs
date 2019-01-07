namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Notification;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class NotificationControllerTests
    {
        private ISessionService sessionService;
        private INotificationService notificationService;

        [Fact]
        public async Task GetNotificationg_WhenCalled_ReturnExpectedNotification()
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

            notificationService.GetNotification(sessionService.EmployeeId, 0, 10).Returns(expectResult);

            // Act
            var actual = await controller.GetNotification(0, 10);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<object>(jsonResult.Value);
            Assert.Equal(expectResult, result);
        }

        [Fact]
        public async Task GetNotificationSetting_WhenCalled_ReturnExpectedSettingList()
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
            notificationService.GetNotificationSetting(sessionService.EmployeeId).Returns(settings);

            // Act
            var actual = await controller.GetNotificationSetting();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<NotificationSetting>>(jsonResult.Value);
            Assert.Equal(settings, result);
        }

        [Fact]
        public async Task UpdateNotificationSetting_WhenCalled_ReturnExpectedResult()
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
            notificationService.UpdateNotificationSetting(sessionService.EmployeeId, settings).Returns(expected);

            // Act
            var actual = await controller.UpdateNotiSetting(settings);

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
            notificationService.ReadNotification(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.ReadNotification(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task MarkAsReadNoti_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };

            var controller = GetNotificationController();
            notificationService.MarkAsReadNotification(sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.MarkAsReadNotification();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 363;

            return service;
        }

        private NotificationController GetNotificationController()
        {
            sessionService = FakeSessionService();
            notificationService = Substitute.For<INotificationService>();
            var controller = new NotificationController(sessionService, notificationService);

            return controller;
        }

        #endregion Helpers
    }
}