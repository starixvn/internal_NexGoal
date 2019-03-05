namespace NexGoal.AutoService.UnitTests.Criterias.Period
{
    using NexGoal.AutoService.Criterias.Period;
    using Xunit;

    public class GeneratePACriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GeneratePACriteria();
            var expected = "Auto_Period_GeneratePA";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string periodIds)
        {
            // Arrange
            var criteria = new GeneratePACriteria { PeriodIds = periodIds };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GeneratePACriteria { PeriodIds = "1,2" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}