namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class SearchUserCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public int? GroupId { get; set; }
        public bool? IsEnabled { get; set; }

        public override string GetSettingKey() => "User_Search";

        public override bool IsValid() => true;
    }
}