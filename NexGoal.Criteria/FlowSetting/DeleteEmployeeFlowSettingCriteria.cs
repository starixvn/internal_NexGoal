namespace NexGoal.Criteria.FlowSetting
{
    using NexGoal.Core.Criteria;

    public class DeleteEmployeeFlowSettingCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "EmployeeFlowSetting_Delete";

        public override bool IsValid() => EmployeeId > 0 && ActorId > 0;
    }
}