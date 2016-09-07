namespace NexGoal.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateEOYCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_Update";
        }

        public override bool IsValid()
        {
            return EOQYId > 0 && ModifiedBy > 0;
        }
    }
}