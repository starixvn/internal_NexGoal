namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;

    public class CreateEmployeeNominationModel
    {
        public int NominationPeriodId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public IEnumerable<KPINominationModel> KpiList { get; set; }
    }
}