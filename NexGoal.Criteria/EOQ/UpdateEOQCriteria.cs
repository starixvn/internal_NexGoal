namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateEOQCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RemindDate { get; set; }

        public DateTime EndDateForCommittee { get; set; }

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