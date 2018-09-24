namespace NexGoal.Models.EmployeeNomination
{
    using System.ComponentModel.DataAnnotations;

    public class CreateEmployeeNominationViewModel
    {
        [Required]
        public int NominationPeriodId { get; set; }

        [Required]
        public int NomineeId { get; set; }

        public string Comment { get; set; }
    }
}