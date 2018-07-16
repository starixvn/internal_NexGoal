namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class SearchUserCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

        public override string GetSettingKey() => "User_Search";

        public override bool IsValid() => true;
    }
}