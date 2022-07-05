namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;

    public class EditEmployeeNominationModel
    {
        public int NominationPeriodId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public string Comment { get; set; }

        public int PerformanceRecordId { get; set; }

        public IEnumerable<KPINominationModel> KpiNominations { get; set; }
    }
}