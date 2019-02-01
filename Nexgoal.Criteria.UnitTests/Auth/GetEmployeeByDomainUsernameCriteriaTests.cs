namespace NexGoal.Criteria.UnitTests.Auth
{
    using NexGoal.Criteria.Auth;
    using Xunit;

    public class GetEmployeeByDomainUsernameCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeByDomainUsernameCriteria();
            var expected = "Employee_GetByDomainUsername";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreInvalid_ReturnFalse()
        {
            // Arrange
            var criteria = new GetEmployeeByDomainUsernameCriteria
            {
                DomainUsername = string.Empty
            };
            var expected = false;
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmployeeByDomainUsernameCriteria
            {
                DomainUsername = "oliver.dang"
            };
            var expected = true;
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(actual, expected);
        }
    }
}