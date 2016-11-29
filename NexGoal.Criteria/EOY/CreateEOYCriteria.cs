namespace NexGoal.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class CreateEOYCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RemindDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_Insert";
        }

        public override bool IsValid()
        {
            return Year > 0 && CreatedBy > 0;
        }
    }
}