namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteNominationCriteria : CriteriaBase
    {
        public int NominationId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQYNomination_Delete";
        }

        public override bool IsValid()
        {
            return NominationId > 0 && ModifiedBy > 0;
        }
    }
}