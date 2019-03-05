namespace NexGoal.AutoService.UnitTests.Criterias.Nomination
{
    using NexGoal.AutoService.Criterias.Nomination;
    using Xunit;

    public class EditWhenFinishedCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditWhenFinishedCriteria();
            var expected = "Auto_Nomination_UpdateWhenFinished";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new EditWhenFinishedCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}