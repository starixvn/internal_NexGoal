namespace NexGoal.BackgroundJob.UnitTests.Criteria.PA
{
    using NexGoal.BackgroundJob.Criteria.PA;
    using Xunit;

    public class EditWhenEvaluationFinishedCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditWhenEvaluationFinishedCriteria();
            var expected = "Auto_PA_UpdateWhenEvaluationFinished";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new EditWhenEvaluationFinishedCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}