namespace NexGoal.Models.PAFocus
{
    public class GetPAFocusParam
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }
        public string GroupIds { get; set; }
        public int? EmployeeId { get; set; }
    }
}