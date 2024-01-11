namespace NexGoal.Core.UnitTests.CustomValidations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Core.CustomValidations;
    using NexGoal.Core.Enumerations;
    using Xunit;

    public class GenericCompareAttributeTests
    {
        [Fact]
        public void IsValid_Valid_ReturnSuccess()
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var model = new GreaterThan { NewValue = 1 };
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Empty(validationResults);
        }

        public static IEnumerable<object[]> Invalid_TestCase()
        {
            yield return new object[] { new GreaterThan { OldValue = 2, NewValue = 1 }, "NewValue must be greater than OldValue." };
            yield return new object[] { new GreaterThanOrEqual { OldValue = 2, NewValue = 1 }, "NewValue must be greater than or equal to OldValue." };
            yield return new object[] { new LessThan { OldValue = 1, NewValue = 2 }, "NewValue must be less than OldValue." };
            yield return new object[] { new LessThanOrEqual { OldValue = 1, NewValue = 2 }, "NewValue must be less than or equal to OldValue." };
        }

        [Theory, MemberData(nameof(Invalid_TestCase))]
        public void IsValid_Invalid_ReturnError(object model, string errorMessage)
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Act
            Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal(errorMessage, validationResults[0].ErrorMessage);
        }

        private class GreaterThan
        {
            public int? OldValue { get; set; }

            [GenericCompare(CompareToPropertyName = nameof(OldValue), Operator = GenericCompareOperator.GreaterThan)]
            public int NewValue { get; set; }
        }

        private class GreaterThanOrEqual
        {
            public int? OldValue { get; set; }

            [GenericCompare(CompareToPropertyName = nameof(OldValue), Operator = GenericCompareOperator.GreaterThanOrEqual)]
            public int NewValue { get; set; }
        }

        private class LessThan
        {
            public int? OldValue { get; set; }

            [GenericCompare(CompareToPropertyName = nameof(OldValue), Operator = GenericCompareOperator.LessThan)]
            public int NewValue { get; set; }
        }

        private class LessThanOrEqual
        {
            public int? OldValue { get; set; }

            [GenericCompare(CompareToPropertyName = nameof(OldValue), Operator = GenericCompareOperator.LessThanOrEqual)]
            public int NewValue { get; set; }
        }
    }
}