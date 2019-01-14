namespace NexGoal.Criteria.NominationReport
{
    using Fanex.Data.Repository;

    public class GetNominationPeriodsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NominationReport_GetPeriods";

        public override bool IsValid() => true;
    }
}