namespace NexGoal.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class CreateEOQCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RemindDate { get; set; }

        public DateTime EndDateForCommittee { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_Insert";
        }

        public override bool IsValid()
        {
            return Quarter > 0 && Year > 0 && CreatedBy > 0;
        }
    }
}