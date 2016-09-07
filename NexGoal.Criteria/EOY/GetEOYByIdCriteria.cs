namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOYByIdCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_GetById";
        }

        public override bool IsValid()
        {
            return EOQYId > 0;
        }
    }
}