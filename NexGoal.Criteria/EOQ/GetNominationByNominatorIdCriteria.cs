namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetNominationByNominatorIdCriteria : CriteriaBase
    {
        public int NominatorId { get; set; }

        public string Query { get; set; }

        public string GroupIds { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQNomination_GetByNominatorId";
        }

        public override bool IsValid()
        {
            return NominatorId > 0;
        }
    }
}