namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetAllPeriodsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Period_GetAll";

        public override bool IsValid() => true;
    }
}