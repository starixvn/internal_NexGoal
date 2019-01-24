namespace NexGoal.AutoService.UnitTests.Criterias
{
    using NexGoal.AutoService.Criterias;
    using Xunit;

    public class CreateNotificationCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateNotificationCriteria();
            var expected = "Notification_Insert";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string notifications)
        {
            // Arrange
            var criteria = new CreateNotificationCriteria { Notifications = notifications };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateNotificationCriteria { Notifications = "xml format" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}