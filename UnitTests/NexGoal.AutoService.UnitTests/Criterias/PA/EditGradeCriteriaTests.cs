namespace NexGoal.AutoService.UnitTests.Criterias.PA
{
    using NexGoal.AutoService.Criterias.PA;
    using Xunit;

    public class EditGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditGradeCriteria();
            var expected = "Auto_PA_UpdateGrade";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string value)
        {
            // Arrange
            var criteria = new EditGradeCriteria { Value = value };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new EditGradeCriteria { Value = "cipherText" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}