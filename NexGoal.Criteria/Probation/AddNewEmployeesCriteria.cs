namespace NexGoal.Criteria.Probation
{
    using NexGoal.Core.Criteria;

    public class AddNewEmployeesCriteria : CommandBaseCriteria
    {
        public string EmployeeIds { get; set; }

        public override string GetSettingKey() => "Probation_InsertNewEmployees";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EmployeeIds);
    }
}