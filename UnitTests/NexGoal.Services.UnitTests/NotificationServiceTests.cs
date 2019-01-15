namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Notification;
    using NexGoal.Models.Common;
    using NexGoal.Models.Notification;
    using NSubstitute;
    using Xunit;

    public class NotificationServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;

        [Fact]
        public async Task GetSettingAsync_WhenCalled_ReturnAsExpectedSetting()
        {
            // Arrange
            var expected = new List<NotificationSetting>
            {
                new NotificationSetting
                {
                    Type = NotificationType.PerformanceAppraisal,
                    ReceiveEmail = true,
                    ReceiveNotification = false
                }
            };
            var service = GetNotificationService();
            baseService.GetDataAsync<NotificationSetting>(Arg.Any<GetNotificationSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetSettingAsync(163);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetNotificationAsync_WhenCalled_ReturnAsExpectedNotification()
        {
            // Arrange
            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    NotificationId = 1,
                    CreatorId = 125,
                    EmployeeId = 263
                }
            };
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 125, NickName="Quinn" },
                new EmployeeBaseModel { EmployeeId = 263, NickName="Tim" },
            };

            var service = GetNotificationService();

            baseService.GetDataAsync<NotificationEntity>(Arg.Any<GetNotificationByEmployeeCriteria>()).Returns(notifications);
            commonService.GetEmployeesAsync().Returns(employees);

            // Act
            var actual = await service.GetNotificationAsync(263, 0, 10);

            // Assert
            Assert.Equal(notifications.Count, actual.Notifications.Count());
        }

        [Fact]
        public async Task EditSettingAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
            var settings = new List<NotificationSetting>
            {
                new NotificationSetting
                {
                    Type = NotificationType.PerformanceAppraisal,
                    ReceiveEmail = true,
                    ReceiveNotification = false
                }
            };
            var service = GetNotificationService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateNotificationSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditSettingAsync(163, settings);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ReadNotificationAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
            var service = GetNotificationService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateNotificationStatusCriteria>()).Returns(expected);

            // Act
            var actual = await service.ReadNotificationAsync(1, 163);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task MarkAsReadAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
            var service = GetNotificationService();
            baseService.ExecuteNonQueryAsync(Arg.Any<MarkAsReadNotificationCriteria>()).Returns(expected);

            // Act
            var actual = await service.MarkAsReadAsync(163);

            // Assert
            Assert.Equal(expected, actual);
        }

        private NotificationService GetNotificationService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            var service = new NotificationService(baseService, commonService);

            return service;
        }
    }
}