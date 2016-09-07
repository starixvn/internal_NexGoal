namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteEOYCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_Delete";
        }

        public override bool IsValid()
        {
            return EOQYId > 0 && ModifiedBy > 0;
        }
    }
}