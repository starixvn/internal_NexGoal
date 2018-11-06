namespace NexGoal.Models.EmployeeNomination
{
    public class NominationModel
    {
        public int NominationId { get; set; }

        public int StatusId { get; set; }

        public int NomineeId { get; set; }

        public int NominationPeriodId { get; set; }

        public NominationPeriodModel Period { get; set; }
    }
}