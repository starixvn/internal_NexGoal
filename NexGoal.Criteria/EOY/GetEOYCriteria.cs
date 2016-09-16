namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOYCriteria : CriteriaBase
    {
        public int? StatusId { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}