namespace NexGoal.Criteria.Authentication
{
    using Fanex.Data.Repository;

    public class GetEmployeeByDomainUsernameCriteria : CriteriaBase
    {
        public string DomainUsername { get; set; }
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DomainUsername);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByDomainUsername";
        }
    }
}