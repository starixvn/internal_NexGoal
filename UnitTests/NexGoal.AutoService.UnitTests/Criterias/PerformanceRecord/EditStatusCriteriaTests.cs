namespace NexGoal.AutoService.UnitTests.Criterias.PerformanceRecord
{
    using NexGoal.AutoService.Criterias.PerformanceRecord;
    using Xunit;

    public class EditStatusCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditStatusCriteria();
            var expected = "EditPerformanceRecordStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new EditStatusCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}