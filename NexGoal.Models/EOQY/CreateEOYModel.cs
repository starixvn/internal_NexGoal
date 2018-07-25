namespace NexGoal.Models.EOQY
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Core.CustomValidations;
    using NexGoal.Core.Enumerations;
    using NexGoal.Resources;

    public class CreateEOYModel
    {
        public int Year { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringMaxLengthErrorMessage))]
        public string Description { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.StartDateForNominator))]
        public DateTime StartDateForNominator { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.EndDateForNominator))]
        [GenericCompare(CompareToPropertyName = "StartDateForNominator", Operator = GenericCompareOperator.GreaterThan)]
        public DateTime EndDateForNominator { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.RemindDateForNominator))]
        [GenericCompare(CompareToPropertyName = "StartDateForNominator", Operator = GenericCompareOperator.GreaterThanOrEqual)]
        [GenericCompare(CompareToPropertyName = "EndDateForNominator", Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime RemindDateForNominator { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.EndDateForCommittee))]
        [GenericCompare(CompareToPropertyName = "EndDateForNominator", Operator = GenericCompareOperator.GreaterThan)]
        public DateTime EndDateForCommittee { get; set; }
    }
}