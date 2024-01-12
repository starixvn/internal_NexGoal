namespace NexGoal.Criteria.UnitTests.Auth
{
    using NexGoal.Criteria.Auth;
    using Xunit;

    public class GetEmployeeInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeInfoCriteria();
            var expected = "Employee_GetInfo";

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
            var criteria = new GetEmployeeInfoCriteria
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
            var criteria = new GetEmployeeInfoCriteria
            {
                EmployeeId = 1
            };
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}