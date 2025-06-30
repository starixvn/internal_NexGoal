namespace NexGoal.BackgroundJob.UnitTests.Criteria.PAFocus
{
    using NexGoal.BackgroundJob.Criteria.PAFocus;
    using Xunit;

    public class GenerateCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GenerateCriteria();
            var expected = "Auto_PAFocus_Generate";

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
            var criteria = new GenerateCriteria { PeriodIds = periodIds };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GenerateCriteria { PeriodIds = "1,2" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}