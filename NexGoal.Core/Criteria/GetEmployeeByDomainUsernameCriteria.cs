namespace NexGoal.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeByDomainUsernameCriteria : CriteriaBase
    {
        public string DomainUsername { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(DomainUsername);

        public override string GetSettingKey() => "Employee_GetByDomainUsername";
    }
}