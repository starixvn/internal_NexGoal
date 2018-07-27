namespace NexGoal.ViewModels.Period
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Core.Constants;

    public class CreatePeriodViewModel : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int TargetTypeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? DisciplineStartDate { get; set; }

        public DateTime? DisciplineEndDate { get; set; }

        public DateTime? FocusStartDate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TargetTypeId == PeriodTarget.SemiAnual)
            {
                if (!StartDate.HasValue)
                {
                    yield return new ValidationResult("Start Date is required", new[] { nameof(StartDate) });
                }

                if (!EndDate.HasValue)
                {
                    yield return new ValidationResult("End Date is required", new[] { nameof(EndDate) });
                }
            }
        }
    }
}