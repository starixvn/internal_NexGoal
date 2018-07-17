namespace NexGoal.Criteria.EOQ
{
    using System;

    public class CreateEOQCriteria : CommandBaseCriteria
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public DateTime StartDateForNominator { get; set; }
        public DateTime EndDateForNominator { get; set; }
        public DateTime RemindDateForNominator { get; set; }
        public DateTime EndDateForCommittee { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "EOQ_Insert";

        public override bool IsValid() => Quarter > 0 && Year > 0 && CreatedBy > 0;
    }
}