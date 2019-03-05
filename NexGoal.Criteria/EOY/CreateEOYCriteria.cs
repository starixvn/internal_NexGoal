namespace NexGoal.Criteria.EOY
{
    using System;
    using NexGoal.Core.Criteria;

    public class CreateEOYCriteria : CommandBaseCriteria
    {
        public int Year { get; set; }
        public string Description { get; set; }
        public DateTime StartDateForNominator { get; set; }
        public DateTime EndDateForNominator { get; set; }
        public DateTime RemindDateForNominator { get; set; }
        public DateTime EndDateForCommittee { get; set; }
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "EOY_Insert";

        public override bool IsValid() => Year > 0 && CreatedBy > 0;
    }
}