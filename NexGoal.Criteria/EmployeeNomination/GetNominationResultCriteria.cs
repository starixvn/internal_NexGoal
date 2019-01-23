namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetNominationResultCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Nomination_GetNominationResult";

        public override bool IsValid() => EmployeeId > 0;
    }
}