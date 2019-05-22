namespace Nexgoal.Criteria.UnitTests.Discipline
{
    using NexGoal.Criteria.Discipline;
    using Xunit;

    public class AddDisciplineCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateSettingCriteria();
            var expected = "Discipline_Insert";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, "")]
        [InlineData(0, 1, "abc")]
        [InlineData(0, 1, "")]
        [InlineData(1, 0, "")]
        [InlineData(1, 1, "")]
        [InlineData(1, 0, "abc")]
        [InlineData(0, 0, "abc")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int empId, int actorId, string description)
        {
            // Arrange
            var criteria = new UpdateSettingCriteria
            {
                EmployeeId = empId,
                CreatedBy = actorId,
                Description = description
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
            var criteria = new UpdateSettingCriteria
            {
                EmployeeId = 1,
                CreatedBy = 2,
                Description = "Test"
            };
            var expected = true;
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(actual, expected);
        }
    }
}