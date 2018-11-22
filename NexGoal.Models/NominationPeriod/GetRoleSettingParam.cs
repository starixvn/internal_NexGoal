namespace NexGoal.Models.NominationPeriod
{
    public class GetRoleSettingParam
    {
        public int NominationPeriodId { get; set; }
        public string Keyword { get; set; }
        public string GroupIds { get; set; }
        public int? RoleId { get; set; }
        public bool IsShowAll { get; set; }
    }
}