namespace NexGoal.ViewModels.Period
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Core.Constants;
    using NexGoal.Resources;

    public class EditPeriodViewModel : IValidatableObject
    {
        public int PeriodId { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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

                if (!DisciplineStartDate.HasValue)
                {
                    yield return new ValidationResult(Message.DisciplineStartDateRequired, new[] { nameof(StartDate) });
                }

                if (!DisciplineEndDate.HasValue)
                {
                    yield return new ValidationResult(Message.DisciplineEndDateRequired, new[] { nameof(EndDate) });
                }

                if (!FocusStartDate.HasValue)
                {
                    yield return new ValidationResult(Message.FocusStartDateRequired, new[] { nameof(EndDate) });
                }
            }
        }
    }
}