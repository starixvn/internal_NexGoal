namespace NexGoal.AutoService.UnitTests.Criterias.PA
{
    using NexGoal.AutoService.Criterias.PA;
    using Xunit;

    public class EditStatusCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditStatusCriteria();
            var expected = "Auto_PA_UpdateStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new EditStatusCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}