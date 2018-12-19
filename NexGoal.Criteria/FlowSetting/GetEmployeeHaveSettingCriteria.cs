namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetEmployeeHaveSettingCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EmployeeFlowSetting_GetEmployeeHaveSetting";
    }
}