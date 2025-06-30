namespace NexGoal.BackgroundJob.UnitTests.Criteria.PA
{
    using NexGoal.BackgroundJob.Criteria.PA;
    using Xunit;

    public class GetDataToEditGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataToEditGradeCriteria();
            var expected = "Auto_PA_GetDataToUpdateGrade";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataToEditGradeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}