namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateEmployeeNominationViewModel
    {
        [Required]
        public int NominationPeriodId { get; set; }

        [Required]
        public int NomineeId { get; set; }

        [Required]
        public IEnumerable<KPINominationModel> KpiNominations { get; set; }
    }
}