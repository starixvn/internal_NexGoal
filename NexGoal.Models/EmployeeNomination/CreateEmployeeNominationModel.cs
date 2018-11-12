namespace NexGoal.Models.EmployeeNomination
{
    public class CreateEmployeeNominationModel
    {
        public int NominationPeriodId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public string Comment { get; set; }

        public int PerformanceRecordStatusId { get; set; }
    }
}