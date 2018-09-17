namespace NexGoal.Criteria.Periods
{
    using System;

    public class CreatePeriodCriteria : CommandBaseCriteria
    {
        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? DisciplineStartDate { get; set; }

        public DateTime? DisciplineEndDate { get; set; }

        public DateTime? FocusStartDate { get; set; }

        public DateTime? FocusEndDate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Period_Insert";

        public override bool IsValid() => !string.IsNullOrEmpty(Name) && CreatedBy > 0 && TargetTypeId > 0;
    }
}