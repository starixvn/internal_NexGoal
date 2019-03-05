namespace NexGoal.AutoService.UnitTests.Criterias.Probation
{
    using NexGoal.AutoService.Criterias.Probation;
    using Xunit;

    public class GetSettingToSendNotificationCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetSettingToSendNotificationCriteria();
            var expected = "Auto_Probation_GetSettingToSendNotification";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetSettingToSendNotificationCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}