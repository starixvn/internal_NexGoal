namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQNominatorSettingByIdCriteria : CriteriaBase
    {
        public int EOQId { get; set; }

        public string Query { get; set; }

        public string GroupIds { get; set; }

        public int? ProcessId { get; set; }

        public bool ShowAllEmployee { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_GetNominatorSettingById";
        }

        public override bool IsValid()
        {
            return EOQId > 0;
        }
    }
}