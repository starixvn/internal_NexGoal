namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusNominationCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int NomineeId { get; set; }

        public int StatusId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQYNomination_UpdateStatus";
        }

        public override bool IsValid()
        {
            return EOQYId > 0 && NomineeId > 0 && StatusId > 0 && ModifiedBy > 0;
        }
    }
}