namespace NexGoal.AutoService.UnitTests.Criterias
{
    using NexGoal.AutoService.Criterias;
    using Xunit;

    public class GetRefResourceByNameCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetRefResourceByNameCriteria();
            var expected = "GetRefResourceByNames";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string refNames)
        {
            // Arrange
            var criteria = new GetRefResourceByNameCriteria { RefNames = refNames };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetRefResourceByNameCriteria { RefNames = "PerformanceGrade" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}