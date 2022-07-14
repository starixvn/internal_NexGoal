namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditEmployeeNominationViewModel
    {
        [Required]
        public int NominationPeriodId { get; set; }

        [Required]
        public int NomineeId { get; set; }

        [Required]
        public int PerformanceRecordId { get; set; }

        public string Comment { get; set; }

        public IEnumerable<KPINominationModel> KpiNominations { get; set; }
    }
}