using NexGoal.Criteria.Common;
using Xunit;

namespace Nexgoal.Criteria.UnitTests.Common
{
    public class GetGroupsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetGroupsCriteria();
            var expected = "Group_GetAll";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Always_ReturnTrue()
        {
            // Arrange
            var criteria = new GetGroupsCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}