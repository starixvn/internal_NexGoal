namespace NexGoal.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetAuthenticationByDomainUsernameCriteria : CriteriaBase
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