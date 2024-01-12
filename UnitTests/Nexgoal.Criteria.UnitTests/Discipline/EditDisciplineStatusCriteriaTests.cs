namespace Nexgoal.Criteria.UnitTests.Discipline
{
    using NexGoal.Criteria.Discipline;
    using Xunit;

    public class EditDisciplineStatusCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateDisciplineStatusCriteria();
            var expected = "Discipline_UpdateStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int disciplineId)
        {
            // Arrange
            var criteria = new UpdateDisciplineStatusCriteria
            {
                DisciplineId = disciplineId
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
            var criteria = new UpdateDisciplineStatusCriteria
            {
                DisciplineId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}