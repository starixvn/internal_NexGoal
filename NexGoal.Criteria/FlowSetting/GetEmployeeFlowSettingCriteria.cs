namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetEmployeeFlowSettingCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "EmployeeFlowSetting_Get";
    }
}