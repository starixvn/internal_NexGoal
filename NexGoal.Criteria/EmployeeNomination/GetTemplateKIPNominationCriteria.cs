namespace NexGoal.Criteria.EmployeeNomination
{
    using Fanex.Data.Repository;

    public class GetTemplateKIPNominationCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "TargetTemplateKPI_Get";

        public override bool IsValid() => true;
    }
}
