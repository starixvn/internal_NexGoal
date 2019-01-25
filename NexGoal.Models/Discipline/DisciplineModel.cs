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

        public Employee Employee { get; set; }

        public Employee Creator { get; set; }
    }
}