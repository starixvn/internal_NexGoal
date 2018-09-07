namespace NexGoal.Models.Period
{
    using System;

    public class EditPeriodModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? DisciplineStartDate { get; set; }

        public DateTime? DisciplineEndDate { get; set; }

        public DateTime? FocusStartDate { get; set; }

        public DateTime? FocusEndDate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }
    }
}