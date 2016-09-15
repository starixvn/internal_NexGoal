namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQNominationByIdCriteria : CriteriaBase
    {
        public int NominationId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQNomination_GetById";
        }

        public override bool IsValid()
        {
            return NominationId > 0;
        }
    }
}