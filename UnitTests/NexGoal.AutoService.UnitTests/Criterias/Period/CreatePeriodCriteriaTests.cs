namespace NexGoal.AutoService.UnitTests.Criterias.Period
{
    using NexGoal.AutoService.Criterias.Period;
    using Xunit;

    public class CreatePeriodCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreatePeriodCriteria();
            var expected = "InsertPeriod";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new CreatePeriodCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}