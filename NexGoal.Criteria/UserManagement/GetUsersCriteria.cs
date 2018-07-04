namespace NexGoal.Criteria.UserManagement
{
    using Fanex.Data.Repository;

    public class GetUsersCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool IsDisabledUser { get; set; }

        public override string GetSettingKey() => "User_Get";

        public override bool IsValid() => true;
    }
}