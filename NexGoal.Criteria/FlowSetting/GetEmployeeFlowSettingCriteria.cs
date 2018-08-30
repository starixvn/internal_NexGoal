namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetEmployeeFlowSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "EmployeeFlowSetting_Get";
    }
}