namespace NexGoal.Core.CustomValidations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Resources;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class GenericCompareAttribute : ValidationAttribute
    {
        public string CompareToPropertyName { get; set; }
        public GenericCompareOperator Operator { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var basePropertyInfo = validationContext.ObjectType.GetProperty(CompareToPropertyName);
            var valOther = (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var valThis = (IComparable)value;

            if (IsGreaterThan(valThis, valOther) ||
                IsGreaterThanOrEqual(valThis, valOther) ||
                IsLessThan(valThis, valOther) ||
                IsLessThanOrEqual(valThis, valOther))
            {
                DisplayAttribute attr = basePropertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true)
                    .Cast<DisplayAttribute>().SingleOrDefault();
                string otherPropertyDisplayName = attr != null ? attr.Name : CompareToPropertyName;

                string message = GetErrorMessage(validationContext.DisplayName, otherPropertyDisplayName);

                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }

        private bool IsGreaterThan(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.GreaterThan && valThis != null && valOther != null && valThis.CompareTo(valOther) <= 0;
        }

        private bool IsGreaterThanOrEqual(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.GreaterThanOrEqual && valThis != null && valOther != null && valThis.CompareTo(valOther) < 0;
        }

        private bool IsLessThan(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.LessThan && valThis != null && valOther != null && valThis.CompareTo(valOther) >= 0;
        }

        private bool IsLessThanOrEqual(IComparable valThis, IComparable valOther)
        {
            return Operator == GenericCompareOperator.LessThanOrEqual && valThis != null && valOther != null && valThis.CompareTo(valOther) > 0;
        }

        private string GetErrorMessage(string displayName, string otherPropertyDisplayName)
        {
            string errorMessage;

            switch (Operator)
            {
                case GenericCompareOperator.GreaterThan:
                    errorMessage = string.Format(Message.GreaterThanCompare, displayName, otherPropertyDisplayName);
                    break;

                case GenericCompareOperator.GreaterThanOrEqual:
                    errorMessage = string.Format(Message.GreaterThanOrEqualCompare, displayName, otherPropertyDisplayName);
                    break;

                case GenericCompareOperator.LessThan:
                    errorMessage = string.Format(Message.LessThanCompare, displayName, otherPropertyDisplayName);
                    break;

                case GenericCompareOperator.LessThanOrEqual:
                    errorMessage = string.Format(Message.LessThanOrEqualCompare, displayName, otherPropertyDisplayName);
                    break;

                default:
                    errorMessage = string.Empty;
                    break;
            }

            return errorMessage;
        }
    }
}