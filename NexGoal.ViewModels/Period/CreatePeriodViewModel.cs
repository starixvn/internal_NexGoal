namespace NexGoal.ViewModels.Period
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Core.Constants;
    using NexGoal.Resources;

    public class CreatePeriodViewModel : IValidatableObject
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.TargetType))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int TargetTypeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? DisciplineStartDate { get; set; }

        public DateTime? DisciplineEndDate { get; set; }

        public DateTime? FocusStartDate { get; set; }

        public DateTime? FocusEndDate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            if (TargetTypeId == TargetType.SemiAnual)
            {
                if (!StartDate.HasValue)
                {
                    yield return new ValidationResult(Message.StartDateRequired, new[] { nameof(StartDate) });
                }

                if (!EndDate.HasValue)
                {
                    yield return new ValidationResult(Message.EndDateRequired, new[] { nameof(EndDate) });
                }

                if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
                {
                    yield return new ValidationResult(Message.StartDateBeforeEndDate, new[] { nameof(StartDate) });
                }

                if (!DisciplineStartDate.HasValue)
                {
                    yield return new ValidationResult(Message.DisciplineStartDateRequired, new[] { nameof(DisciplineStartDate) });
                }

                if (!DisciplineEndDate.HasValue)
                {
                    yield return new ValidationResult(Message.DisciplineEndDateRequired, new[] { nameof(DisciplineEndDate) });
                }

                if (DisciplineStartDate.HasValue && DisciplineEndDate.HasValue && DisciplineStartDate > DisciplineEndDate)
                {
                    yield return new ValidationResult(Message.DisciplineStartDateBeforeDisciplineEndDate, new[] { nameof(DisciplineStartDate) });
                }

                if (!FocusStartDate.HasValue)
                {
                    yield return new ValidationResult(Message.FocusStartDateRequired, new[] { nameof(FocusStartDate) });
                }

                if (!FocusEndDate.HasValue)
                {
                    yield return new ValidationResult(Message.FocusEndDateRequired, new[] { nameof(FocusEndDate) });
                }

                if (FocusStartDate.HasValue && FocusEndDate.HasValue && FocusStartDate > FocusEndDate)
                {
                    yield return new ValidationResult(Message.FocusStartDateBeforeFocusEndDate, new[] { nameof(FocusStartDate) });
                }
            }
        }
    }
}