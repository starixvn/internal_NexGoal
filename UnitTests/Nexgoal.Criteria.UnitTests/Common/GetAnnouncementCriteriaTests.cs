namespace Nexgoal.Criteria.UnitTests.Common
{
    using NexGoal.Criteria.Common;
    using Xunit;

    public class GetAnnouncementCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAnnouncementCriteria();
            var expected = "Announcement_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int empId)
        {
            // Arrange
            var criteria = new GetAnnouncementCriteria
            {
                EmployeeId = empId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAnnouncementCriteria { EmployeeId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}