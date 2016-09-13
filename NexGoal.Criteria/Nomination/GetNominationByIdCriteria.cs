namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetNominationByIdCriteria : CriteriaBase
    {
        public int NominationId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQYNomination_GetById";
        }

        public override bool IsValid()
        {
            return NominationId > 0;
        }
    }
}