namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateEOYCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RemindDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_Update";
        }

        public override bool IsValid()
        {
            return Year > 0 && EOQYId > 0 && ModifiedBy > 0;
        }
    }
}