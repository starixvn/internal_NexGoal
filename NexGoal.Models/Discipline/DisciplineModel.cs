namespace NexGoal.Models.Discipline
{
    using System;
    using NexGoal.Models.Common;

    public class DisciplineModel
    {
        public int DisciplineId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime EffectiveDate { get; set; }

        public EmployeeBaseModel Employee { get; set; }

        public EmployeeBaseModel Creator { get; set; }
    }
}