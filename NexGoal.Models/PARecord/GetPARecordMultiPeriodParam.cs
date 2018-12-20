namespace NexGoal.Models.PARecord
{
    public class GetPARecordMultiPeriodParam
    {
        public int ActorId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

        public int? EmployeeId { get; set; }
    }
}