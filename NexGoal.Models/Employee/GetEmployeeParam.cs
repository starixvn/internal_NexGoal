namespace NexGoal.Models.Employee
{
    public class GetEmployeeParam
    {
        public string Keyword { get; set; }
        public int? GroupId { get; set; }
        public string RoleIds { get; set; }
        public bool? IsDisabled { get; set; }
    }
}