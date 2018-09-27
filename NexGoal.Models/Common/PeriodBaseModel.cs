namespace NexGoal.Models.Common
{
    using System;

    public class PeriodBaseModel
    {
        public virtual int PeriodId { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}