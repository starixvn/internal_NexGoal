namespace NexGoal.Model.Violation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NexGoal.Model.Enitites;

    public class ViolationViewModel
    {
        public int ViolationId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime EffectedDate { get; set; }

        public Employee Employee { get; set; }

        public Employee CreatedBy { get; set; }
    }
}
