namespace NexGoal.Models.PARecord
{
    public class GetPARecordParam
    {
        public int ActorId { get; set; }

        public int PeriodId { get; set; }

        public string GroupIds { get; set; }

        public bool? IsEdited { get; set; }
    }
}