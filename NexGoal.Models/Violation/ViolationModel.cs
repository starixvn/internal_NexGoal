namespace NexGoal.Model.Violation
{
    using NexGoal.Model.Enitites;
    using System;

    public class ViolationViewModel
    {
        public int ViolationId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime EffectedDate { get; set; }

        public string GroupName { get; set; }

        public Employee Employee { get; set; }

        public Employee CreatedBy { get; set; }
    }
}
