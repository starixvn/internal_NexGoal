namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQNominationByNominatorIdCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public int NominatorId { get; set; }

        public int RoleId { get; set; }

        public string Query { get; set; }

        public string GroupIds { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_CurrentNomination_GetByNominatorId";
        }

        public override bool IsValid()
        {
            return NominatorId > 0;
        }
    }
}