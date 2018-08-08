namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetEmployeeDoNotHaveSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }
        public int? GroupId { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "EmployeeFlowSetting_GetEmployeeDoNotHaveSetting";
    }
}