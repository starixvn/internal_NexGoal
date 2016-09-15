namespace NexGoal.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateEOQCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_Update";
        }

        public override bool IsValid()
        {
            return Quarter > 0 && Year > 0 && ModifiedBy > 0;
        }
    }
}