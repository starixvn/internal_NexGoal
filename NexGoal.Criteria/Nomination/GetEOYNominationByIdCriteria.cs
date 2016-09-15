namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOYNominationByIdCriteria : CriteriaBase
    {
        public int NominationId { get; set; }

        public override string GetSettingKey()
        {
            return "EOYNomination_GetById";
        }

        public override bool IsValid()
        {
            return NominationId > 0;
        }
    }
}