namespace Nexgoal.Criteria.UnitTests.Discipline
{
    using NexGoal.Criteria.Discipline;
    using Xunit;

    public class EditDisciplineCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditDisciplineCriteria();
            var expected = "Discipline_Update";

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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int disciplineId, int actorId, string description)
        {
            // Arrange
            var criteria = new EditDisciplineCriteria
            {
                DisciplineId = disciplineId,
                ModifiedBy = actorId,
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
            var criteria = new EditDisciplineCriteria
            {
                ModifiedBy = 1,
                DisciplineId = 2,
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