namespace Nexgoal.Criteria.UnitTests.Common
{
    using NexGoal.Criteria.Common;
    using Xunit;

    public class GetProjectsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetProjectsCriteria();
            var expected = "Project_GetAll";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Always_ReturnTrue()
        {
            // Arrange
            var criteria = new GetProjectsCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}