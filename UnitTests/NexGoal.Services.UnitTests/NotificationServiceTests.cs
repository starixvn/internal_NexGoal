namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Notification;
    using NexGoal.Models.Notification;
    using NSubstitute;
    using Xunit;

    public class NotificationServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetNotificationSetting_WhenCalled_ReturnAsExpectedSetting()
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
            var actual = await service.GetNotificationSetting(163);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdateNotificationSetting_WhenCalled_ReturnAsExpectedCommand()
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
            var actual = await service.UpdateNotificationSetting(163, settings);

            // Assert
            Assert.Equal(expected, actual);
        }

        private NotificationService GetNotificationService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new NotificationService(baseService);

            return service;
        }
    }
}