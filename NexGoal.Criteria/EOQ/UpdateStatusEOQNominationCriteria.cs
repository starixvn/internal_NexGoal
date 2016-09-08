namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusEOQNominationCriteria : CriteriaBase
    {
        public int NominationId { get; set; }

        public int StatusId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQNomination_UpdateStatus";
        }

        public override bool IsValid()
        {
            return NominationId > 0 && StatusId > 0 && ModifiedBy > 0;
        }
    }
}