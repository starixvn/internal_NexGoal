namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Attribute;
    using System;

    public class CreateEOQCriteria : CriteriaBase
    {
        [Output]
        public int EOQYId { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

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