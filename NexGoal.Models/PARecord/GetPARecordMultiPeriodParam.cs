namespace NexGoal.Models.PARecord
{
    public class GetPARecordMultiPeriodParam
    {
        public int ActorId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

        public string EmployeeIds { get; set; }

        public bool? IsRevised { get; set; }
    }
}