namespace NexGoal.Models.Period
{
    using System;

    public class PeriodModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public string TargetType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? DisciplineStartDate { get; set; }

        public DateTime? DisciplineEndDate { get; set; }

        public DateTime? FocusStartDate { get; set; }

        public DateTime? FocusEndDate { get; set; }

        public int? TotalEmployees { get; set; }

        public int? TotalUnfinished { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }
    }
}