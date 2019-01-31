namespace Nexgoal.Criteria.UnitTests.Common
{
    using NexGoal.Criteria.Common;
    using Xunit;

    public class SearchEmployeeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new SearchEmployeeCriteria();
            var expected = "Employee_Search";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Always_ReturnTrue()
        {
            // Arrange
            var criteria = new SearchEmployeeCriteria();
            var expected = true;
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(actual, expected);
        }
    }
}