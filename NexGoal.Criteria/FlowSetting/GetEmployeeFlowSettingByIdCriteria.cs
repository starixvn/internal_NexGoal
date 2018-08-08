namespace NexGoal.Criteria.FlowSetting
{
    using Fanex.Data.Repository;

    public class GetEmployeeFlowSettingByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public int FlowTemplateId { get; set; }

        public override string GetSettingKey() => "EmployeeFlowSetting_GetById";

        public override bool IsValid() => EmployeeId > 0 && FlowTemplateId > 0;
    }
}