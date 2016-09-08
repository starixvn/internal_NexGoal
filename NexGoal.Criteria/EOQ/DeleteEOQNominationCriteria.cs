namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteEOQNominationCriteria : CriteriaBase
    {
        public int NominationId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQNomination_Delete";
        }

        public override bool IsValid()
        {
            return NominationId > 0 && ModifiedBy > 0;
        }
    }
}