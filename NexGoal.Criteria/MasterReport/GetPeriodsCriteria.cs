namespace NexGoal.Criteria.MasterReport
{
    using Fanex.Data.Repository;

    public class GetPeriodsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "MasterReport_GetPeriods";

        public override bool IsValid() => true;
    }
}